using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trackable.Repositories
{
    public interface IRepository<TKey, TModel>
    {
        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="model">The model to be added.</param>
        /// <returns>The async task.</returns>
        Task<TModel> AddAsync(TModel model);

        /// <summary>
        /// Adds models asynchronously.
        /// </summary>
        /// <param name="models">The model enumerable to be added.</param>
        /// <returns>The async task.</returns>
        Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models);

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="id">The model ID to be deleted.</param>
        /// <returns>The async task.</returns>
        Task DeleteAsync(TKey id);

        /// <summary>
        /// Gets all models asynchronously.
        /// </summary>
        /// <returns>The models async task.</returns>
        Task<IEnumerable<TModel>> GetAllAsync();

        /// <summary>
        /// Gets a model asynchronously by ID.
        /// </summary>
        /// <param name="id">The model ID.</param>
        /// <returns>The model async task.</returns>
        Task<TModel> GetAsync(TKey id);

        /// <summary>
        /// Updates a model asynchronously.
        /// </summary>
        /// <param name="id">The model ID.</param>
        /// <param name="model">The model to be updated.</param>
        /// <returns>The model async task.</returns>
        Task<TModel> UpdateAsync(TKey id, TModel model);

        /// <summary>
        /// Updates models asynchronously.
        /// </summary>
        /// <param name="models">The model dictionary to be updated.</param>
        /// <returns>The model enumerable async task.</returns>
        Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models);
    }

    public interface IRepository<TKey1, TKey2, TModel>
    {
        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="model">The model to be added.</param>
        /// <returns>The async task.</returns>
        Task<TModel> AddAsync(TModel model);

        /// <summary>
        /// Adds a model asynchronously.
        /// </summary>
        /// <param name="models">The model enumerable to be added.</param>
        /// <returns>The async task.</returns>
        Task AddAsync(IEnumerable<TModel> models);

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="id">The model ID to be deleted.</param>
        /// <returns>The async task.</returns>
        Task DeleteAsync(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Gets all models asynchronously.
        /// </summary>
        /// <returns>The models async task.</returns>
        Task<IEnumerable<TModel>> GetAllAsync();

        /// <summary>
        /// Gets a model asynchronously by ID.
        /// </summary>
        /// <param name="id">The model ID.</param>
        /// <returns>The model async task.</returns>
        Task<TModel> GetAsync(TKey1 key1, TKey2 key2);

        /// <summary>
        /// Updates a model asynchronously.
        /// </summary>
        /// <param name="id">The model ID.</param>
        /// <param name="model">The model to be updated.</param>
        /// <returns>The async task.</returns>
        Task<TModel> UpdateAsync(TKey1 key1, TKey2 key2, TModel model);
    }
}