using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;
using System;
using System.Linq.Expressions;
using AutoMapper;

namespace Trackable.Repositories
{
    internal class LocationRepository : DbRepositoryBase<int, LocationData, Location>, ILocationRepository
    {

        public LocationRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<IEnumerable<Location>> GetAsync(IEnumerable<int> ids)
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

        public async Task<IDictionary<string, int>> GetCountPerAssetAsync(int locationId)
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
