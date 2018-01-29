using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    class GeoFenceUpdateRepository : DbRepositoryBase<int, GeoFenceUpdateData, GeoFenceUpdate>, IGeoFenceUpdateRepository
    {
        public GeoFenceUpdateRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<IDictionary<int, GeoFenceUpdate>> GetLatestAsync(string assetId)
        {
            return await this.FindBy(n => n.AssetDataId == assetId)
                .GroupBy(n => n.GeoFenceDataId)
                .ToDictionaryAsync( g => g.Key, g => g.AsQueryable()
                    .OrderByDescending(n => n.CreatedAtTimeUtc)
                    .AsEnumerable()
                    .Select(d => this.ObjectMapper.Map<GeoFenceUpdate>(d))
                    .FirstOrDefault());
        }

        protected override Expression<Func<GeoFenceUpdateData, object>>[] Includes => null;

    }
}
