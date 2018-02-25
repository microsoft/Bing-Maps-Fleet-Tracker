using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the location repository.
    /// </summary>
    public interface ILocationRepository : 
        IRepository<int, Location>, 
        IDbCountableRepository<int, LocationData, Location>,
        IDbNamedRepository<int, LocationData, Location>,
        IDbTaggedRepository<int, LocationData, Location>
    {
        Task<IEnumerable<Location>> GetAsync(IEnumerable<int> ids);

        Task<IDictionary<string, int>> GetCountPerAssetAsync(int locationId);

        Task<int> GetAutoLocationCountAsync();
    }
}
