// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    abstract class CrudServiceBase<TKey, TModel, TRepository> : ICrudService<TKey, TModel> 
        where TModel : ModelBase<TKey>
        where TRepository : IRepository<TKey, TModel>
    {
        protected readonly TRepository repository;

        public CrudServiceBase(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models)
        {
            return await this.repository.AddAsync(models);
        }

        public virtual async Task<TModel> AddAsync(TModel model)
        {
            return await this.repository.AddAsync(model);
        }

        public virtual async Task DeleteAsync(TKey key)
        {
            await this.repository.DeleteAsync(key);
        }

        public virtual async Task<TModel> GetAsync(TKey key)
        {
            return await this.repository.GetAsync(key);
        }

        public virtual async Task<IEnumerable<TModel>> ListAsync()
        {
            return await this.repository.GetAllAsync();
        }

        public virtual async Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models)
        {
            return await this.repository.UpdateAsync(models);
        }

        public virtual async Task<TModel> UpdateAsync(TKey key, TModel model)
        {
            return await this.repository.UpdateAsync(key, model);
        }
    }
}
