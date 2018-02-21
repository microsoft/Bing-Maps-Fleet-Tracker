using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITripService : ICrudService<int, Trip>
    {
        Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId);
    }
}
