using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class TripService : CrudServiceBase<int, Trip, ITripRepository>, ITripService
    {
        public TripService(ITripRepository repository)
            : base(repository)
        {
        }

        public Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId)
        {
            return this.repository.GetByAssetIdAsync(assetId);
        }
    }
}
