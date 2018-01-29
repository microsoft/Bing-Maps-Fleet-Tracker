using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;
using System;
using AutoMapper;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<GeoFence>> GetByAssetIdAsync(string assetId)
        {
            var data = await this.FindBy(g => g.AssetDatas.Select(a => a.Id).Contains(assetId))
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<GeoFence>(d));
        }

        public async Task<int> GetCountAsync()
        {
            return await this.FindBy(a => true).CountAsync();
        }

        protected override Expression<Func<GeoFenceData, object>>[] Includes => new Expression<Func<GeoFenceData, object>>[]
        {
            data => data.AssetDatas
        };
    }
}
