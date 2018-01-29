using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the tracking device repository.
    /// </summary>
    public interface ITrackingDeviceRepository : IRepository<string, TrackingDevice>, ICountableRepository
    {
        Task<TrackingDevice> GetDeviceByNameAsync(string name);
        Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions();
    }
}