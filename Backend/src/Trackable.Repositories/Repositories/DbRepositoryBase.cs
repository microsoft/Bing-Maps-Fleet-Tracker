using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.EntityFramework;
using Trackable.Models;
using Trackable.Models.Helpers;
using Trackable.Common.Exceptions;
using AutoMapper;

namespace Trackable.Repositories
{
    internal abstract class DbRepositoryBase<TKey, TData, TModel>
        : IRepository<TKey, TModel>
        where TData : EntityBase<TKey>
        where TModel : ModelBase<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the model mapper.
        /// </summary>
        //protected IMapper (AutoMapper)
        protected IMapper ObjectMapper { get; set; }

        /// <summary>
        /// The relations to be loaded with query
        /// </summary>
        protected abstract Expression<Func<TData, object>>[] Includes { get; }

        /// <summary>
        /// The database context.
        /// </summary>
        protected TrackableDbContext Db { get; }

        /// <summary>
        /// Constructs new instance of the RepositoryBase.
        /// </summary>
        /// <param name="db">The db context.</param>
        /// <param name="mapper">The model mapper.</param>
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

            if(this.Includes != null)
            {
                data = await this.FindAsync(data.Id);
            }

            return ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="models">The business models.</param>
        /// <returns>The async task.</returns>
        public virtual async Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models)
        {
            models.ThrowIfNull(nameof(models));

            if (!models.Any())
            {
                return Enumerable.Empty<TModel>();
            }

            var dataModels = models.Select(m => ObjectMapper.Map<TData>(m));

            var resultingData = this.Db.Set<TData>().AddRange(dataModels);

            await this.Db.SaveChangesAsync();

            return resultingData.Select(d => ObjectMapper.Map <TModel>(d));
        }

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The async task.</returns>
        public async Task DeleteAsync(TKey id)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new ResourceNotFoundException("Attempting to delete a resource that does not exist");
            }

            data.Deleted = true;
            await this.Db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a model asynchronously by ID.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The model async task.</returns>
        public async Task<TModel> GetAsync(TKey id)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                return default(TModel);
            }

            return ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Gets all models asynchronously.
        /// </summary>
        /// <returns>The models async task.</returns>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var data = await this.FindBy(t => true).ToListAsync();

            return data.Select(d => this.ObjectMapper.Map<TModel>(d));
        }

        /// <summary>
        /// Updates a model asynchronously.
        /// </summary>
        /// <param name="model">The model to be updated.</param>
        /// <returns>The async task.</returns>
        public virtual async Task<TModel> UpdateAsync(TKey id, TModel model)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new ResourceNotFoundException("Attempting to update a resource that does not exist");
            }

            UpdateData(data, model);

            await this.Db.SaveChangesAsync();

            if (this.Includes != null)
            {
                data = await this.FindAsync(data.Id);
            }
            
            return ObjectMapper.Map<TModel>(data);
        }

        /// <summary>
        /// Updates an enumerable of models asynchronously.
        /// </summary>
        /// <param name="model">The models to be updated.</param>
        /// <returns>The async task.</returns>
        public virtual async Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models)
        {
            var dataModels = await this.FindBy(item => models.Keys.Contains(item.Id)).ToListAsync();

            if (dataModels.Any(d => d == null))
            {
                throw new ResourceNotFoundException("Attempting to update a resource that does not exist");
            }

            var data = dataModels.ToDictionary(i => i.Id, i => i);

            foreach (var item in data)
            {
                UpdateData(item.Value, models[item.Key]);
            }

            await this.Db.UpdateAsync(data.Select(d => d.Value));

            if (this.Includes != null)
            {
                var ids = data.Select(d => d.Key).ToList();
                var res = await this.FindBy(d => ids.Contains(d.Id)).ToListAsync();
                return res.Select(d => this.ObjectMapper.Map<TModel>(d));
            }

            return data.Select(i => this.ObjectMapper.Map<TModel>(i.Value));
        }

        /// <summary>
        /// Finds a data model by key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<TData> FindAsync(TKey id)
        {
            return await this.FindBy(data => data.Id.Equals(id)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Returns a queryable of valid elements after applying the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The found queryable.</returns>
        protected IQueryable<TData> FindBy(Expression<Func<TData, bool>> predicate)
        {
            var query = this.Db.Set<TData>()
                .Where(d => !d.Deleted)
                .Where(predicate);

            if (this.Includes != null)
            {
                query = this.Includes.Aggregate(query, (current, include) => current.Include(include));
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

            // Exclude CreatedAt property
            dataProperties = dataProperties.Where(p => p.Name != "CreatedAtTimeUtc").ToArray();

            foreach (var property in dataProperties)
            {
                property.SetValue(data, property.GetValue(intermediaryData));
            }
        }
    }
}
