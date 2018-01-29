using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class LocationService : CrudServiceBase<int, Location, ILocationRepository>, ILocationService
    {
        public LocationService(ILocationRepository repository)
            : base(repository)
        {
        }

        public async Task<IDictionary<string, int>> GetCountByAssetAsync(int locationid)
        {
            return await this.repository.GetCountPerAssetAsync(locationid);
        }
    }
}
