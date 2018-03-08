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

        public async override Task<Asset> AddAsync(Asset model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new BadArgumentException("Asset must have a name");
            }

            var nameIsUsed = await this.FindBy(a => a.Name == model.Name).AnyAsync();
            if (nameIsUsed)
            {
                throw new BadArgumentException("Asset name must be unique");
            }

            return await base.AddAsync(model);
        }

        public async override Task<IEnumerable<Asset>> AddAsync(IEnumerable<Asset> models)
        {
            foreach (var model in models)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString("N");
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new BadArgumentException("Assets must have a name");
                }
            }

            var modelNames = models.Select(m => m.Name).ToList();
            var nameIsUsed = await this.FindBy(a => modelNames.Contains(a.Name)).AnyAsync();
            if (nameIsUsed)
            {
                throw new BadArgumentException("Asset names must be unique");
            }

            return await base.AddAsync(models);
        }

        public async Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions()
        {
            return await this.Db.Assets
                .Where(d => !d.Deleted && d.LatestPosition != null)
                .Include(d => d.LatestPosition)
                .ToDictionaryAsync(d => d.Name, d => this.ObjectMapper.Map<TrackingPoint>(d.LatestPosition));
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
