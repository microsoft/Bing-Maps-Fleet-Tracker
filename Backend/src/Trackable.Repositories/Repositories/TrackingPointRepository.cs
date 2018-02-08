using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    internal class TrackingPointRepository : DbRepositoryBase<int, TrackingPointData, TrackingPoint>, ITrackingPointRepository
    {
        public TrackingPointRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override async Task<TrackingPoint> AddAsync(TrackingPoint model)
        {
            var device = await this.Db.TrackingDevices
                .Where(d => !d.Deleted)
                .Include(d => d.Asset)
                .SingleAsync(d => d.Id == model.TrackingDeviceId);

            if (device.Asset == null)
            {
                throw new InvalidOperationException("Can't add a tracking point while device not linked to an asset");
            }

            model.AssetId = device.Asset.Id;

            return await this.AddAsyncInternal(model, device);
        }

        public override async Task<IEnumerable<TrackingPoint>> AddAsync(IEnumerable<TrackingPoint> models)
        {
            if (!models.Any())
            {
                return new List<TrackingPoint>();
            }

            string deviceId = models.First().TrackingDeviceId;

            var device = await this.Db.TrackingDevices
                .Where(d => !d.Deleted)
                .Include(d => d.Asset)
                .SingleAsync(d => d.Id == deviceId);

            if (device.Asset == null)
            {
                throw new InvalidOperationException("Can't add a tracking point while device not linked to an asset");
            }

            models.ForEach(model => model.AssetId = device.Asset.Id);

            var orderedModels = models.OrderBy(p => p.DeviceTimestampUtc);
            var savedModels = (await base.AddAsync(orderedModels.Take(models.Count() - 1))).ToList();
            var latestPoint = await this.AddAsyncInternal(orderedModels.Last(), device);

            savedModels.Add(latestPoint);

            return savedModels;
        }

        public async Task<IEnumerable<TrackingPoint>> GetByAssetIdAsync(string assetId)
        {
            var data = await this
                .FindBy(a => a.AssetId == assetId)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<TrackingPoint>(d));
        }

        public async Task<IEnumerable<TrackingPoint>> GetByDeviceIdAsync(string deviceId)
        {
            var data = await this
                .FindBy(a => a.TrackingDeviceId == deviceId)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<TrackingPoint>(d));
        }

        public async Task<IEnumerable<TrackingPoint>> GetByAssetIdAfterDateAsync(string assetId, DateTime date, bool includeDebug)
        {
            var data = await this
                .FindBy(a => a.CreatedAtTimeUtc > date && a.AssetId == assetId && (includeDebug || !includeDebug && !a.Debug))
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<TrackingPoint>(d));
        }

        public async Task<TrackingPoint> GetByAssetIdLastLabeledAsync(string assetId)
        {
            var data = await this
                .FindBy(a => a.TripId != null && a.AssetId == assetId)
                .OrderByDescending(a => a.CreatedAtTimeUtc)
                .FirstOrDefaultAsync();
            return this.ObjectMapper.Map<TrackingPoint>(data);
        }

        public async Task<IEnumerable<TrackingPoint>> GetNearestPoints(int tripId, IPoint point, int count)
        {
            var data = await this.FindBy(a => a.TripId == tripId)
                .OrderBy(p => p.Location.Distance(
                        DbGeography.PointFromText("POINT(" + point.Longitude + " " + point.Latitude + ")", 4326)))
                .Take(count)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<TrackingPoint>(d));
        }

        public async Task AssignPointsToTripAsync(int tripId, IEnumerable<TrackingPoint> points)
        {
            var dataPoints = new List<TrackingPointData>();
            foreach (var p in points)
            {
                p.TripId = tripId;
                dataPoints.Add(this.ObjectMapper.Map<TrackingPointData>(p));
            }

            await this.Db.UpdateAsync(dataPoints, "TripId");
        }

        public async Task<IDictionary<string, int>> PointsPerAssetCountAsync()
        {
            return await this.FindBy(a => true)
                .GroupBy(p => p.AssetId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<IDictionary<DateTime, int>> PointsPerDayCountAsync()
        {
            return await this.FindBy(a => true)
                .GroupBy(p => DbFunctions.TruncateTime(p.CreatedAtTimeUtc))
                .ToDictionaryAsync(g => g.Key.Value, g => g.Count());
        }

        public async Task<int> GetCountAsync()
        {
            return await this.FindBy(a => true).CountAsync();
        }

        protected override Expression<Func<TrackingPointData, object>>[] Includes => new Expression<Func<TrackingPointData, object>>[]
        {
            data => data.Asset
        };

        private async Task<TrackingPoint> AddAsyncInternal(TrackingPoint trackingPoint, TrackingDeviceData deviceData)
        {
            var data = this.ObjectMapper.Map<TrackingPointData>(trackingPoint);

            var addedData = this.Db.Set<TrackingPointData>().Add(data);

            deviceData.LatestPosition = addedData;
            deviceData.Asset.LatestPosition = addedData;

            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<TrackingPoint>(await this.FindAsync(addedData.Id));
        }
    }
}
