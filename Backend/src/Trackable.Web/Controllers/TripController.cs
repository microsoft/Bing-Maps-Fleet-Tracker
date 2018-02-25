using AutoMapper;
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
using Trackable.Web.DTOs;

namespace Trackable.Web.Controllers
{
    [Route("api/trip")]
    public class TripController : ControllerBase
    {
        private readonly IPipeline pipeline;
        private readonly ITripDetectorFactory tripFactory;
        private readonly IAssetService assetService;
        private readonly ITrackingPointService pointService;
        private readonly IMapper dtoMapper;

        public TripController(
            ITripDetectorFactory tripFactory,
            IAssetService assetService,
            ITrackingPointService pointService,
            ILoggerFactory loggerFactory,
            IPipeline pipeline,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.pipeline = pipeline;
            this.tripFactory = tripFactory;
            this.assetService = assetService;
            this.pointService = pointService;
            this.dtoMapper = dtoMapper;
        }

        [HttpGet("detect")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IDictionary<string, IEnumerable<TripDto>>> Detect()
        {
            var tripDetector = await tripFactory.Create();

            var assetIds = (await this.assetService.ListAsync()).Select(a => a.Id);
            var dict = new Dictionary<string, IEnumerable<Trip>>();

            foreach (var assetId in assetIds)
            {
                var result = await pipeline.ExecuteModules(tripDetector.GetModuleLoaders(), assetId);
                dict.Add(assetId, (IEnumerable<Trip>)result);
            }

            return this.dtoMapper.Map<IDictionary<string, IEnumerable<TripDto>>>(dict);
        }

        // GET api/trip/5/points?lat=x&lon=y
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPointDto>> GetPoint(int id, double lat, double lon, int count = 5)
        {
            var results = await this.pointService.GetNearestPoints(id, new Point(lat, lon), count);

            return this.dtoMapper.Map<IEnumerable<TrackingPointDto>>(results);
        }
    }
}
