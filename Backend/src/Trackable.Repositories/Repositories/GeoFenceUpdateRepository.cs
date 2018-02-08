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
    class GeoFenceUpdateRepository : DbRepositoryBase<int, GeoFenceUpdateData, GeoFenceUpdate>, IGeoFenceUpdateRepository
    {
        public GeoFenceUpdateRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<IDictionary<int, GeoFenceUpdate>> GetByGeofenceIdsAsync(string assetId, IEnumerable<int> geofenceIds)
        {
            return await this.Db.GeoFenceUpdates
                .Where(g => g.AssetDataId == assetId && geofenceIds.Contains(g.GeoFenceDataId))
                .ToDictionaryAsync(r => r.GeoFenceDataId, r => this.ObjectMapper.Map<GeoFenceUpdate>(r));
        }

        public async Task UpdateStatusAsync(int geofenceId, NotificationStatus status)
        {
            var geofenceUpdate = await this.Db.GeoFenceUpdates.SingleAsync(g => geofenceId == g.Id);

            if (geofenceUpdate == null)
            {
                return;
            }

            geofenceUpdate.Status = (int)status;
            geofenceUpdate.CreatedAtTimeUtc = DateTime.UtcNow;

            await this.Db.SaveChangesAsync();
        }

        protected override Expression<Func<GeoFenceUpdateData, object>>[] Includes => null;

    }
}
