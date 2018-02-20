using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;

namespace Trackable.Web.Controllers
{
    [Route("api/points")]
    public class PointsController : ControllerBase
    {
        private readonly ITrackingPointService pointService;
        private readonly IGeoFenceService geoFenceService;

        public PointsController(
            ITrackingPointService pointService,
            IGeoFenceService geoFenceService,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.pointService = pointService.ThrowIfNull(nameof(pointService));
            this.geoFenceService = geoFenceService.ThrowIfNull(nameof(geoFenceService));
        }

        // POST api/points
        [HttpPost]
        [Authorize(UserRoles.Viewer)]
        public async Task<IActionResult> PostPoint([FromBody]TrackingPoint point)
        {
            if (string.IsNullOrEmpty(point.TrackingDeviceId))
            {
                return BadRequest("Point must include a TrackingDeviceId");
            }

            var addedPoint = await this.pointService.AddAsync(point);
            await this.geoFenceService.HandlePoints(addedPoint.AssetId, addedPoint);

            return Ok();
        }

        // POST api/points/batch
        [HttpPost("batch")]
        [Authorize(UserRoles.Viewer)]
        public async Task<IActionResult> PostPoints([FromBody]TrackingPoint[] points)
        {
            var pointsWithoutId = points.Where((point) => string.IsNullOrEmpty(point.TrackingDeviceId));
            if (pointsWithoutId.Any())
            {
                return BadRequest("Points must include a TrackingDeviceId");
            }

            var addedPoints = await this.pointService.AddAsync(points);
            var pointsLookup = addedPoints.ToLookup(a => a.AssetId, a => a);

            foreach (var pl in pointsLookup)
            {
                await this.geoFenceService.HandlePoints(pl.Key, pl.ToArray());
            }

            return Ok();
        }
    }
}
