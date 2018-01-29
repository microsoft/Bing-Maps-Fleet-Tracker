using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.EntityFramework;
using Trackable.Models.Helpers;

namespace Trackable.Repositories
{
    internal abstract class DbRepositoryBase<TKey1, TKey2, TData, TModel> : IRepository<TKey1, TKey2, TModel> 
        where TData : EntityBase<TKey1, TKey2>
        where TKey1 : IEquatable<TKey1>
        where TKey2 : IEquatable<TKey2>
    {

        /// <summary>
        /// Gets the model converter.
        /// </summary>
        protected IMapper ObjectMapper { get; set; }

        protected TrackableDbContext Db { get; }

        public DbRepositoryBase(TrackableDbContext db, IMapper mapper)
        {
            this.ObjectMapper = mapper.ThrowIfNull(nameof(mapper));
            this.Db = db.ThrowIfNull(nameof(db));
        }

        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="model">The business model.</param>
        /// <returns>The async task.</returns>
        public virtual async Task<TModel> AddAsync(TModel model)
        {
            model.ThrowIfNull(nameof(model));

            var data = this.ObjectMapper.Map<TData>(model);

            this.Db.Set<TData>().Add(data);

            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="models">The business models.</param>
        /// <returns>The async task.</returns>
        public virtual async Task AddAsync(IEnumerable<TModel> models)
        {
            models.ThrowIfNull(nameof(models));

            this.Db.Set<TData>().AddRange(
                models.Select(m => this.ObjectMapper.Map<TData>(m)));

            await this.Db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2)
        {
            var data = await this.FindAsync(key1, key2);
            data.Deleted = true;
            await this.Db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a model asynchronously by ID.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The model async task.</returns>
        public async Task<TModel> GetAsync(TKey1 key1, TKey2 key2)
        {
            var data = await this.FindAsync(key1, key2);

            if (data == null)
            {
                return default(TModel);
            }

            return this.ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Gets all models asynchronously.
        /// </summary>
        /// <returns>The models async task.</returns>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var data = await this.Db.Set<TData>()
                .Where(d => !d.Deleted)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<TModel>(d));
        }

        /// <summary>
        /// Updates a model asynchronously.
        /// </summary>
        /// <param name="model">The model to be updated.</param>
        /// <returns>The async task.</returns>
        public async Task<TModel> UpdateAsync(TKey1 key1, TKey2 key2, TModel model)
        {
            var data = await this.FindAsync(key1, key2);
            UpdateData(data, model);
            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Finds a data model by key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<TData> FindAsync(TKey1 key1, TKey2 key2)
        {
            return await this.FindBy(data => data.Key1.Equals(key1) && data.Key2.Equals(key2)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Returns a queryable of valid elements after applying the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The found queryable.</returns>
        protected IQueryable<TData> FindBy(Expression<Func<TData, bool>> predicate, params Expression<Func<TData, object>>[] includes)
        {
            var query = this.Db.Set<TData>()
                .Where(d => !d.Deleted)
                .Where(predicate);

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }

        /// <summary>
        /// Updates the data model with the required properties from the business model.
        /// </summary>
        /// <param name="data">The data model.</param>
        /// <param name="model">The business model.</param>
        public void UpdateData(TData data, TModel model)
        {
            var clonedModel = this.ObjectMapper.Map<TModel>(data);

            var modelProperties = typeof(TModel).GetProperties();

            foreach (var property in modelProperties)
            {
                var attributes = property.GetCustomAttributes();
                if (attributes.Any(a => a.GetType() == typeof(MutableAttribute)))
                {
                    property.SetValue(clonedModel, property.GetValue(model));
                }
            }

            var intermediaryData = this.ObjectMapper.Map<TData>(clonedModel);

            var dataProperties = typeof(TData).GetProperties();
            foreach (var property in dataProperties)
            {
                property.SetValue(data, property.GetValue(intermediaryData));
            }
        }
    }
}
