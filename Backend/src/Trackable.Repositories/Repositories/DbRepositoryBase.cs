// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;
using Trackable.Models.Helpers;

namespace Trackable.Repositories
{
    public abstract class DbRepositoryBase<TKey, TData, TModel>
        : IRepository<TKey, TModel>, IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>
        where TModel : ModelBase<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the model mapper.
        /// </summary>
        //protected IMapper (AutoMapper)
        public IMapper ObjectMapper { get; set; }

        /// <summary>
        /// The relations to be loaded with query
        /// </summary>
        protected abstract Expression<Func<TData, object>>[] Includes { get; }

        /// <summary>
        /// The database context.
        /// </summary>
        protected TrackableDbContext Db { get; }

        /// <summary>
        /// A pointer to the dbRepository for running extensions queries
        /// </summary>
        public DbRepositoryBase<TKey, TData, TModel> DbBaseRepository => this;

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

            if (this.Includes != null)
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

            // Unless passed data models is an array, EF will not fill computed fields
            var dataModels = models.Select(m => ObjectMapper.Map<TData>(m)).ToArray();

            var resultingData = this.Db.Set<TData>().AddRange(dataModels);

            await this.Db.SaveChangesAsync();

            return ObjectMapper.Map<IEnumerable<TModel>>(resultingData);
        }

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The async task.</returns>
        public virtual async Task DeleteAsync(TKey id)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new ResourceNotFoundException("Attempting to delete a resource that does not exist");
            }

            this.Db.Set<TData>().Attach(data);
            data.Deleted = true;

            await this.Db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a model asynchronously by ID.
        /// </summary>
        /// <param name="model">The model ID.</param>
        /// <returns>The model async task.</returns>
        public virtual async Task<TModel> GetAsync(TKey id)
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
        public virtual async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var data = await this.FindBy(t => true).ToListAsync();

            return this.ObjectMapper.Map<IEnumerable<TModel>>(data);
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

            this.Db.Set<TData>().Attach(data);

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
            var dataModels = await this.FindBy(d => models.Keys.Contains(d.Id)).ToListAsync();

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
                return this.ObjectMapper.Map<IEnumerable<TModel>>(res);
            }

            return this.ObjectMapper.Map<IEnumerable<TModel>>(data);
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
        public IQueryable<TData> FindBy(Expression<Func<TData, bool>> predicate)
        {
            var dbQuery = this.Db.Set<TData>()
                .AsNoTracking()
                .Where(d => !d.Deleted)
                .Where(predicate);

            if (this.Includes != null)
            {
                dbQuery = this.Includes.Aggregate(dbQuery, (current, include) => current.Include(include));
            }

            return dbQuery;
        }

        /// <summary>
        /// Updates the data model with the required properties from the business model.
        /// </summary>
        /// <param name="data">The data model.</param>
        /// <param name="model">The business model.</param>
        protected void UpdateData(TData data, TModel model)
        {
            // Generate business model from original data model
            var clonedModel = this.ObjectMapper.Map<TModel>(data);

            // Mutate business model properties that have the mutable attribute
            var modelProperties = typeof(TModel).GetProperties();
            foreach (var property in modelProperties)
            {
                var attributes = property.GetCustomAttributes();
                if (attributes.Any(a => a.GetType() == typeof(MutableAttribute)))
                {
                    property.SetValue(clonedModel, property.GetValue(model));
                }
            }

            // Data model generated from mutated business model
            var intermediaryData = this.ObjectMapper.Map<TData>(clonedModel);

            // Get the names of properties mapped by automapper (model => data)
            var propertyMap = this.ObjectMapper.ConfigurationProvider.FindTypeMapFor<TModel, TData>();
            var mappedPropertyNames = propertyMap.GetPropertyMaps().Where(m => m.IsMapped()).Select(m => m.DestinationProperty.Name);

            // Include only data model properties mapped by auto mapper
            var dataProperties = typeof(TData).GetProperties();
            dataProperties = dataProperties.Where(p => mappedPropertyNames.Contains(p.Name)).ToArray();

            // Update data model values
            foreach (var property in dataProperties)
            {
                property.SetValue(data, property.GetValue(intermediaryData));
            }
        }
    }
}
