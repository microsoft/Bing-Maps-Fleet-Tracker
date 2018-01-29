using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            
        // Do not save points if they are debug points
        if (models.All(p => p.Debug))
        {
            results = models;
            var device = await this.deviceRepository.GetAsync(models.First().TrackingDeviceId);
            foreach(var point in results)
            {
                point.AssetId = device?.AssetId;
            }
        }
        else
        {
            results = await base.AddAsync(models);
        }

        var pointsList = results.OrderBy(p => p.DeviceTimestampUtc);
        var deviceId = pointsList.Last().TrackingDeviceId;
        var assetId = pointsList.Last().AssetId;

        return results;
    }
}