// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;

namespace Trackable.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString, string rootPath)
        {
            TrackableDbContext.LoadSqlServerTypes(Path.Combine(rootPath, "bin"));

            return services.AddScoped(provider => new TrackableDbContext(connectionString));
        }

        public static IApplicationBuilder UseDb(this IApplicationBuilder builder, string connectionString)
        {
            var configuration = new Migrations.Configuration
            {
                TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient")
            };

            var migrator = new DbMigrator(configuration);
            migrator.Update();

            return builder;
        }
    }
}
