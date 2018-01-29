using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the trip repository.
    /// </summary>
    public interface ITripRepository : IRepository<int, Trip>
    {
        /// <summary>
        /// Gets all trips by asset ID asynchronously.
        /// </summary>
        /// <param name="assetId">The asset ID.</param>
        /// <returns>The trips async task.</returns>
        Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId);
    }
}
