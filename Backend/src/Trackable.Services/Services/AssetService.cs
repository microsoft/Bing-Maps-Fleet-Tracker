using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class AssetService : CrudServiceBase<string, Asset, IAssetRepository>, IAssetService
    {
        public AssetService(IAssetRepository repository)
            : base(repository)
        {
        }

        public async override Task<Asset> AddAsync (Asset asset)
        {
            if(asset.AssetType == AssetType.Car && asset.AssetProperties != null)
            {
                throw new BadArgumentException("AssetType \"Car\" should not contain assetProperties");
            }

            return await this.repository.AddAsync(asset);
        }

        public async Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions()
        {
            return await this.repository.GetAssetsLatestPositions();
        }
    }
}
