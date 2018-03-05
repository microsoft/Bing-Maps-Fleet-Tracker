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
    internal class TripRepository : DbRepositoryBase<int, TripData, Trip>, ITripRepository
    {
        public TripRepository(
            TrackableDbContext db,
            IMapper mapper)
            : base(db, mapper)
        {
        }
        
        public async Task<IEnumerable<Trip>> GetByAssetIdAsync(string assetId)
        {
            var data = await this
                .FindBy(a => a.AssetId == assetId)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<Trip>(d));
        }

        protected override Expression<Func<TripData, object>>[] Includes => new Expression<Func<TripData, object>>[]
        {
            data => data.StartLocation,
            data => data.EndLocation,
            data => data.TripLegDatas
        };
    }
}
