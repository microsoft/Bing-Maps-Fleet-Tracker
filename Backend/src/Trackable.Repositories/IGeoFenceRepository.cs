using Trackable.EntityFramework;
using Trackable.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the Geofence repository.
    /// </summary>
    public interface IGeoFenceRepository : IRepository<int, GeoFence>, ICountableRepository
    {
        Task<IEnumerable<GeoFence>> GetByAssetIdAsync(string assetId);

        Task<GeoFence> UpdateAssetsAsync(GeoFence fence, IEnumerable<string> assetIds);
    }
}
