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
    class GeoFenceRepository : DbRepositoryBase<int, GeoFenceData, GeoFence>, IGeoFenceRepository
    {
        public GeoFenceRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<GeoFence> UpdateAssetsAsync(GeoFence fence, IEnumerable<string> assetIds)
        {
            var assets = await this.Db.Assets.Where(a => assetIds.Contains(a.Id)).ToListAsync();
            var fenceData = await this.FindAsync(fence.Id);

            fenceData.AssetDatas.Clear();

            foreach (var asset in assets)
            {
                fenceData.AssetDatas.Add(asset);
            }

            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<GeoFence>(fenceData);
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
