// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
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

        public async override Task<IEnumerable<Asset>> AddAsync(IEnumerable<Asset> assets)
        {
            if (assets.Any(asset => asset.AssetType == AssetType.Car && asset.AssetProperties != null))
            {
                throw new BadArgumentException("AssetType \"Car\" should not contain assetProperties");
            }

            return await this.repository.AddAsync(assets);
        }

        public async Task<IEnumerable<Asset>> FindContainingAllTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAllTagsAsync(tags);
        }

        public async Task<IEnumerable<Asset>> FindContainingAnyTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAnyTagsAsync(tags);
        }

        public async Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions()
        {
            return await this.repository.GetAssetsLatestPositions();
        }
    }
}
