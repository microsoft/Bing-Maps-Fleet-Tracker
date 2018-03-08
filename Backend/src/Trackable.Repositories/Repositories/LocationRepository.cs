// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    internal class LocationRepository : DbRepositoryBase<string, LocationData, Location>, ILocationRepository
    {

        public LocationRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override Task<IEnumerable<Location>> AddAsync(IEnumerable<Location> models)
        {
            foreach (var model in models)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString("N");
                }
            }

            return base.AddAsync(models);
        }

        public override Task<Location> AddAsync(Location model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            return base.AddAsync(model);
        }

        public async Task<IEnumerable<Location>> GetAsync(IEnumerable<string> ids)
        {
            var data = await this.FindBy(l => ids.Contains(l.Id))
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<Location>(data));
        }

        public async Task<int> GetAutoLocationCountAsync()
        {
            return await this.FindBy(a => a.InterestLevel.HasValue &&
                    (a.InterestLevel.Value == (int)InterestLevel.AutoNew ||
                    a.InterestLevel.Value == (int)InterestLevel.AutoHigh ||
                    a.InterestLevel.Value == (int)InterestLevel.AutoLow)
                ).CountAsync();
        }

        public async Task<IDictionary<string, int>> GetCountPerAssetAsync(string locationId)
        {
            return await this.Db.TripLegs
                .Include(leg => leg.Trip)
                .Where(leg => leg.StartLocationId == locationId || leg.EndLocationId == locationId)
                .GroupBy(leg => leg.Trip.AssetId)
                .OrderByDescending(g => g.Count())
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        protected override Expression<Func<LocationData, object>>[] Includes => new Expression<Func<LocationData, object>>[]
        {
            data => data.Tags
        };
    }
}
