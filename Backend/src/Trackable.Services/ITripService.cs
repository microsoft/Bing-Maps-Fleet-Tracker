using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITripService : ICrudService<int, Trip>
    {
        Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId);
    }

    class TripService : CrudServiceBase<int, Trip, ITripRepository>, ITripService
    {
        public TripService(ITripRepository repository)
            :base(repository)
        {
        }

        public Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId)
        {
            return this.repository.GetByAssetIdAsync(assetId);
        }
    }
}
