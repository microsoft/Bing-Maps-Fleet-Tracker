using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface IAssetPropertiesRepository : IRepository<int, AssetProperties>
    {
        Task CheckValidity(AssetProperties assetProperties);
    }
}
