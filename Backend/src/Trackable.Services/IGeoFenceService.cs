using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IGeoFenceService : ICrudService<int, GeoFence>
    {
        Task<IEnumerable<int>> HandlePoints(string assetId, params IPoint[] points);

        Task<IEnumerable<GeoFence>> FindByNameAsync(string name);

        Task<IEnumerable<GeoFence>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<GeoFence>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
