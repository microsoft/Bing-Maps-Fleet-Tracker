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
    class AssetRepository : DbRepositoryBase<string, AssetData, Asset>, IAssetRepository
    {
        public AssetRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
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
            asset => asset.AssetProperties
        };

        public async Task<int> GetCountAsync()
        {
            return await this.FindBy(a => true).CountAsync();
        }
    }
}
