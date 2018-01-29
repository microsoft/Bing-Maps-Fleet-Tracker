using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trackable.EntityFramework;

namespace Trackable.Repositories
{
    public static class DbContextBulkOperations
    {
        private static string GenericTypeCreateSQL = @"
            IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = '{0}')
            CREATE TYPE {0} AS TABLE
            (
	            {1},
	            {2}
            );
        ";

        private static string GenericProcedureCreateSQL = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('{0}'))
               exec('CREATE PROCEDURE {0} AS BEGIN SET NOCOUNT ON; END')
            ";

        private static string GenericProcedureAlterSQL = @"
            ALTER PROCEDURE {0}
	            @TVP {1} READONLY
            AS
            BEGIN
                MERGE INTO {2} AS T
                USING @TVP AS S
                ON {3}
                WHEN MATCHED THEN UPDATE SET {4};
            END
        ";


        /// <summary>
        /// Bulk updates the specified fields in the supplied items.
        /// </summary>
        /// <typeparam name="TEntity">The type of the items to be updated</typeparam>
        /// <param name="context">The database context used to update</param>
        /// <param name="items">List of items that will be updated</param>
        /// <param name="fieldsToUpdate">Fields in type TEntity that will be updated. Leave empty to update all fields</param>
        /// <returns></returns>
        public static async Task UpdateAsync<TEntity>(
            this DbContext context,
            IEnumerable<TEntity> items,
            params string[] fieldsToUpdate) where TEntity : class
        {
            var objectType = ObjectContext.GetObjectType(typeof(TEntity));

            var tableName = context.GetTableName<TEntity>();
            var primaryKeyNames = context.GetPrimaryKeyNames<TEntity>();

            var primaryKeyProperties = objectType
                .GetProperties()
                .Where(p => primaryKeyNames.Contains(p.Name));

            var fieldProperties = objectType
                .GetProperties()
                .Where(p => !primaryKeyNames.Contains(p.Name));
          
            if(fieldsToUpdate != null && fieldsToUpdate.Any())
            {
                fieldProperties = fieldProperties.Where(f => fieldsToUpdate.Contains(f.Name));
            }

            var rows = new List<List<Object>>();
            foreach (var item in items)
            {
                var row = new List<object>();
                row.AddRange(primaryKeyProperties.Select(f => f.GetValue(item)));
                row.AddRange(fieldProperties.Select(f => f.GetValue(item)));

                rows.Add(row);
            }

            await UpdateAsync(
                context,
                tableName,
                primaryKeyProperties.ToDictionary(f => f.Name, f => f.PropertyType),
                fieldProperties.ToDictionary(f => f.Name, f => f.PropertyType),
                rows);
        }

        private static async Task UpdateAsync(
            DbContext context,
            string tableName,
            IDictionary<string, Type> idFields,
            IDictionary<string, Type> fieldsToUpdate,
            IEnumerable<List<Object>> rows)
        {
            var table = new DataTable();

            // Id fields are guaranteed to be primitives
            foreach (var idField in idFields)
            {
                table.Columns.Add(idField.Key, idField.Value);
            }

            var primitiveFields = new Dictionary<string, Type>();
            var excludedColumns = new List<int>();
            int index = idFields.Count;
            foreach (var column in fieldsToUpdate)
            {
                // DataTable does not support Nullable types, instead primitive types should be added
                // and the column marked as AllowDbNull
                if (column.Value.IsGenericType && column.Value.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    DataColumn col = new DataColumn(column.Key, column.Value.GetGenericArguments().First());
                    col.AllowDBNull = true;
                    table.Columns.Add(col);

                    primitiveFields.Add(column.Key, column.Value.GetGenericArguments().First());
                }
                else if(SqlTypeHelper.GetDbType(column.Value).HasValue)
                {
                    table.Columns.Add(column.Key, column.Value);
                    primitiveFields.Add(column.Key, column.Value);
                }
                else
                {
                    // Column is unsupported type. Do not include it.
                    excludedColumns.Add(index);
                }

                index++; 
            }

            excludedColumns.Reverse();

            foreach (var row in rows)
            {
                foreach(var col in excludedColumns)
                {
                    row.RemoveAt(col);
                }

                table.Rows.Add(row.ToArray());
            }

            var connectionInitialState = context.Database.Connection.State;

            try
            {
                if (connectionInitialState != ConnectionState.Open)
                {
                    await context.Database.Connection.OpenAsync();
                }

                var typeName = await ExecuteTypeSqlCommand(context, tableName, idFields, primitiveFields);
                var storedProcedureName = await ExecuteStoredProcedureSqlCommand(context, tableName, typeName, idFields, primitiveFields);

                await ExecuteUpdateAsync(context, table, storedProcedureName, typeName);
            }
            finally
            {
                if (connectionInitialState != ConnectionState.Open)
                {
                    context.Database.Connection.Close();
                }
            }

        }

        private async static Task ExecuteUpdateAsync(DbContext context, DataTable table, string storedProcedureName, string typeName)
        {
            using (var command = context.Database.Connection.CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                var tableParameter = new SqlParameter("TVP", table);
                tableParameter.SqlDbType = SqlDbType.Structured;
                tableParameter.TypeName = typeName;

                command.Parameters.Add(tableParameter);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async static Task<string> ExecuteTypeSqlCommand(
            DbContext context,
            string tableName,
            IDictionary<string, Type> idFields,
            IDictionary<string, Type> fieldsToUpdate)
        {
            using (var command = context.Database.Connection.CreateCommand())
            {
                var typeName = GetUniqueTypeName(tableName, fieldsToUpdate);

                var idFieldsString =
                   string.Join(",", idFields.Select(f => $"{f.Key} {SqlTypeHelper.GetDbType(f.Value).ToString()}"));

                var fieldsToUpdateBuilder = new StringBuilder();
                foreach (var fieldToUpdate in fieldsToUpdate)
                {
                    var sqlType = SqlTypeHelper.GetDbType(fieldToUpdate.Value);
                    var sqlTypeString = sqlType.ToString();
                    if(sqlType == SqlDbType.NVarChar)
                    {
                        sqlTypeString += "(MAX)";
                    }
                    
                    fieldsToUpdateBuilder.Append($"{fieldToUpdate.Key} {sqlTypeString},");
                }

                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(
                    GenericTypeCreateSQL,
                    typeName,
                    idFieldsString,
                    fieldsToUpdateBuilder.ToString(0, fieldsToUpdateBuilder.Length - 1));

                await command.ExecuteNonQueryAsync();
                return typeName;
            }
        }

        private async static Task<string> ExecuteStoredProcedureSqlCommand(
            DbContext context,
            string tableName,
            string typeName,
            IDictionary<string, Type> idFields,
            IDictionary<string, Type> fieldsToUpdate)
        {
            var procedureName = $"sp_{typeName}";

            using (var command = context.Database.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(
                    GenericProcedureCreateSQL,
                    procedureName);

                await command.ExecuteNonQueryAsync();
            }

            using (var command = context.Database.Connection.CreateCommand())
            {
                var fieldsToUpdateString =
                    string.Join(",", fieldsToUpdate.Select(f => $"T.{f.Key} = S.{f.Key}"));

                var idFieldsString =
                    string.Join(",", idFields.Select(f => $"T.{f.Key} = S.{f.Key}"));

                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(
                    GenericProcedureAlterSQL,
                    procedureName,
                    typeName,
                    tableName,
                    idFieldsString,
                    fieldsToUpdateString);

                await command.ExecuteNonQueryAsync();
            }

            return procedureName;
        }

        private static string GetUniqueTypeName(string tableName, IDictionary<string, Type> fieldsToUpdate)
        {
            var uniqueName = $"{tableName}_";
            foreach (var f in fieldsToUpdate)
            {
                uniqueName += f.Key;
            }

            return uniqueName;
        }

        public static string GetTableName<TEntity>(this DbContext context) where TEntity : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.GetTableName<TEntity>();
        }

        public static string GetTableName<TEntity>(this ObjectContext context) where TEntity : class
        {
            string sql = context.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM\\s+\\[.*\\]\\.\\[(?<table>.*)\\]\\s+AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        public static IEnumerable<string> GetPrimaryKeyNames<TEntity>(this DbContext context) where TEntity : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            ObjectSet<TEntity> set = objectContext.CreateObjectSet<TEntity>();
            return set.EntitySet.ElementType
                .KeyMembers
                .Select(k => k.Name);
        }
    }
}
