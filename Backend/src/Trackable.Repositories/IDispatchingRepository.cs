using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface IDispatchingRepository : IRepository<int, Dispatch>
    {
        Task<IEnumerable<Dispatch>> GetByDeviceIdAsync(string deviceId);
    }
}
