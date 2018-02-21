using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ILocationService : ICrudService<int, Location>
    {
        Task<IDictionary<string, int>> GetCountByAssetAsync(int locationid);

        Task<IEnumerable<Location>> FindByNameAsync(string name);

        Task<IEnumerable<Location>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<Location>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
