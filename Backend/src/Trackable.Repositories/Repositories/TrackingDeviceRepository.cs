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
    class TrackingDeviceRepository : DbRepositoryBase<string, TrackingDeviceData, TrackingDevice>, ITrackingDeviceRepository
    {
        public TrackingDeviceRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<TrackingDevice> GetDeviceByNameAsync(string name)
        {
            var data = await this.FindBy(device => (device.Name == name)).SingleOrDefaultAsync();

            if (data == null)
            {
                return default(TrackingDevice);
            }
            else
            {
                return this.ObjectMapper.Map<TrackingDevice>(data);
            }
        }


        public async Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions()
        {
            return await this.Db.TrackingDevices
                .Where(d => !d.Deleted)
                .Include(d => d.LatestPosition)
                .ToDictionaryAsync(d => d.Name, d => this.ObjectMapper.Map<TrackingPoint>(d.LatestPosition));
        }

        protected override Expression<Func<TrackingDeviceData, object>>[] Includes => new Expression<Func<TrackingDeviceData, object>>[]
        {
            data => data.Asset,
            data => data.Tags
        };
    }
}
