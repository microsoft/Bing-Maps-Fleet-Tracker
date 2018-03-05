// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    class AssetRepository : DbRepositoryBase<string, AssetData, Asset>, IAssetRepository
    {
        public AssetRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async override Task<Asset> UpdateAsync(string id, Asset model)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new ResourceNotFoundException("Attempting to update a resource that does not exist");
            }

            data.Tags = model.Tags.Select(t => new TagData { TagName = t }).ToList();

            await this.Db.SaveChangesAsync();

            return ObjectMapper.Map<Asset>(await this.FindAsync(data.Id));
        }

        public async override Task<IEnumerable<Asset>> UpdateAsync(IDictionary<string, Asset> models)
        {
            var dataModels = await this.FindBy(item => models.Keys.Contains(item.Id)).ToListAsync();

            if (dataModels.Any(d => d == null))
            {
                throw new ResourceNotFoundException("Attempting to update a resource that does not exist");
            }

            foreach (var item in dataModels)
            {
                item.Tags = models[item.Id].Tags.Select(t => new TagData { TagName = t }).ToList();
            }

            await this.Db.SaveChangesAsync();

            var ids = models.Select(d => d.Key).ToList();
            return this.ObjectMapper.Map<IEnumerable<Asset>>(await this.FindBy(d => ids.Contains(d.Id)).ToListAsync());
        }

        public async Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions()
        {
            return await this.Db.Assets
                .Where(d => !d.Deleted)
                .Include(d => d.LatestPosition)
                .ToDictionaryAsync(d => d.Id, d => this.ObjectMapper.Map<TrackingPoint>(d.LatestPosition));
        }

        public async Task<int> GetNumberOfActiveAssets(DateTime activeThreshold)
        {
            return await this.Db.Assets
                .Where(d => (!d.Deleted && d.LatestPosition.CreatedAtTimeUtc > activeThreshold))
                .CountAsync();
        }

        protected override Expression<Func<AssetData, object>>[] Includes => new Expression<Func<AssetData, object>>[]
        {
            asset => asset.TrackingDevice,
            asset => asset.AssetProperties,
            asset => asset.Tags
        };
    }
}
