using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.TripDetection;

namespace Trackable.Web.Controllers
{
    [Route("api/trip")]
    public class TripController : ControllerBase
    {
        private readonly IPipeline pipeline;
        private readonly ITripDetectorFactory tripFactory;
        private readonly IAssetService assetService;
        private readonly ITrackingPointService pointService;
        private readonly ITripService tripService;

        public TripController(
            ITripDetectorFactory tripFactory,
            IAssetService assetService,
            ITripService tripService,
            ITrackingPointService pointService,
            ILoggerFactory loggerFactory,
            IPipeline pipeline)
            : base(loggerFactory)
        {
            this.pipeline = pipeline.ThrowIfNull(nameof(pipeline));
            this.tripFactory = tripFactory.ThrowIfNull(nameof(tripFactory));
            this.assetService = assetService.ThrowIfNull(nameof(assetService));
            this.pointService = pointService.ThrowIfNull(nameof(pointService));
            this.tripService = tripService.ThrowIfNull(nameof(tripService));
        }

        [HttpGet("detect")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IDictionary<string, IEnumerable<Trip>>> Detect()
        {
            var tripDetector = await tripFactory.Create();

            var assetIds = (await this.assetService.ListAsync()).Select(a => a.Id);
            var dict = new Dictionary<string, IEnumerable<Trip>>();

            foreach (var assetId in assetIds)
            {
                var result = await pipeline.ExecuteModules(tripDetector.GetModuleLoaders(), assetId);
                dict.Add(assetId, (IEnumerable<Trip>)result);
            }

            return dict;
        }

        // GET api/trip/5/points?lat=x&lon=y
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPoint>> GetPoint(int id, double lat, double lon, int count = 5)
        {
            return await this.pointService.GetNearestPoints(id, new Point(lat, lon), count);
        }
    }
}
