using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the location repository.
    /// </summary>
    public interface ILocationRepository : IRepository<int, Location> , ICountableRepository
    {
        Task<IEnumerable<Location>> GetAsync(IEnumerable<int> ids);

        Task<IDictionary<string, int>> GetCountPerAssetAsync(int locationId);

        Task<int> GetAutoLocationCountAsync();
    }
}
