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
using Trackable.Repositories.Helpers;

namespace Trackable.Repositories
{
    class GeoFenceRepository : DbRepositoryBase<string, GeoFenceData, GeoFence>, IGeoFenceRepository
    {
        public GeoFenceRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override Task<GeoFence> AddAsync(GeoFence model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            return base.AddAsync(model);
        }

        public override Task<IEnumerable<GeoFence>> AddAsync(IEnumerable<GeoFence> models)
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

        public async Task<Dictionary<GeoFence, bool>> GetByAssetIdWithIntersectionAsync(string assetId, IPoint[] points)
        {
            var pointsGeography = GeographyHelper.CreateDbMultiPoint(points);

            return await this.FindBy(g => g.AssetDatas.Any(a => a.Id == assetId))
                 .Select(g => new
                 {
                     GeoFence = g,
                     Intersects = g.AreaType == (int)GeoFenceAreaType.Polygon ? g.Polygon.Intersects(pointsGeography) :
                        g.AreaType == (int)GeoFenceAreaType.Circular ? g.Polygon.Distance(pointsGeography) < g.Radius : false
                 })
                 .ToDictionaryAsync(r => this.ObjectMapper.Map<GeoFence>(r.GeoFence), r => r.Intersects);
        }

        protected override Expression<Func<GeoFenceData, object>>[] Includes => new Expression<Func<GeoFenceData, object>>[]
        {
            data => data.AssetDatas,
            data => data.Tags
        };
    }
}
