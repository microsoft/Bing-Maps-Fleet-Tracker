using Trackable.EntityFramework;
using Trackable.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the asset repository.
    /// </summary>
    public interface IGeoFenceUpdateRepository : IRepository<int, GeoFenceUpdate>
    {
        Task<IDictionary<int, GeoFenceUpdate>> GetByGeofenceIdsAsync(string assetId, IEnumerable<int> geofenceIds);

        Task UpdateStatusAsync(int geofenceId, NotificationStatus status);
    }

}
