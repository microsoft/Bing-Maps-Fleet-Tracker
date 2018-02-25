using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Repositories;
using Trackable.Services;

class TrackingPointService : CrudServiceBase<int, TrackingPoint, ITrackingPointRepository>, ITrackingPointService
{
    private readonly ILogger logger;
    private ITrackingDeviceRepository deviceRepository;

    public TrackingPointService(
        ITrackingPointRepository repository,
        ITrackingDeviceRepository deviceRepository,
        ILoggerFactory loggerFactory)
        : base(repository)
    {
        this.logger = loggerFactory.CreateLogger<TrackingPointService>();
        this.deviceRepository = deviceRepository;
    }

    public Task<IEnumerable<TrackingPoint>> GetByAssetIdAsync(string assetId)
    {
        return this.repository.GetByAssetIdAsync(assetId);
    }

    public Task<IEnumerable<TrackingPoint>> GetByDeviceIdAsync(string deviceId)
    {
        return this.repository.GetByDeviceIdAsync(deviceId);
    }

    public Task<IEnumerable<TrackingPoint>> GetNearestPoints(int tripId, IPoint point, int count)
    {
        return this.repository.GetNearestPoints(tripId, point, count);
    }

    public async override Task<IEnumerable<TrackingPoint>> AddAsync(IEnumerable<TrackingPoint> models)
    {
        IEnumerable<TrackingPoint> results;

        // Do not save points if they are debug points, but still retrieve asset id
        if (models.All(p => p.Debug))
        {
            var devicePointsLookup = models.ToLookup(m => m.TrackingDeviceId);

            results = models;

            foreach (var dpl in devicePointsLookup)
            {
                var device = await this.deviceRepository.GetAsync(dpl.Key);
                foreach (var point in dpl)
                {
                    point.AssetId = device?.AssetId;
                }
            }
        }
        else
        {
            results = await base.AddAsync(models);
        }

        return results;
    }
}