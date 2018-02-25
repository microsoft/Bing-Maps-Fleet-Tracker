using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IAssetService : ICrudService<string, Asset>
    {
        Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions();

        Task<IEnumerable<Asset>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<Asset>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
