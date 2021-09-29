// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trackable.Common;
using Trackable.Models;
using Trackable.Web.Dtos;
using Trackable.Services;

namespace Trackable.Web.Controllers
{
    [Route("api/points")]
    public class PointsController : ControllerBase
    {
        private readonly ITrackingPointService pointService;
        private readonly IGeoFenceService geoFenceService;
        private readonly IMapper dtoMapper;

        public PointsController(
            ITrackingPointService pointService,
            IGeoFenceService geoFenceService,
            ILoggerFactory loggerFactory,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.geoFenceService = geoFenceService;
            this.pointService = pointService;
            this.dtoMapper = dtoMapper;
        }

        /// <summary>
        /// Create TrackingPoint and check geofences
        /// </summary>
        /// <param name="point">The tracking point details</param>
        /// <returns>Ok response</returns>
        // POST api/points
        [HttpPost]
        [Authorize(UserRoles.Viewer)]
        public async Task<IActionResult> PostPoint([FromBody]TrackingPointDto point)
        {
            if (string.IsNullOrEmpty(point.TrackingDeviceId))
            {
                return BadRequest("Point must include a TrackingDeviceId");
            }

            var model = this.dtoMapper.Map<TrackingPoint>(point);

            var addedPoint = await this.pointService.AddAsync(model);
            await this.geoFenceService.HandlePoints(addedPoint.AssetId, addedPoint);

            return Ok();
        }

        /// <summary>
        /// Create multiple TrackingPoints and check geofences
        /// </summary>
        /// <param name="points">List of TrackingPoint details</param>
        /// <returns>Ok response</returns>
        // POST api/points/batch
        [HttpPost("batch")]
        [Authorize(UserRoles.Viewer)]
        public async Task<IActionResult> PostPoints([FromBody]TrackingPointDto[] points)
        {
            var pointsWithoutId = points.Where((point) => string.IsNullOrEmpty(point.TrackingDeviceId));
            if (pointsWithoutId.Any())
            {
                return BadRequest("Points must include a TrackingDeviceId");
            }

            var models = this.dtoMapper.Map<TrackingPoint[]>(points);

            var addedPoints = await this.pointService.AddAsync(models);
            var pointsLookup = addedPoints.ToLookup(a => a.AssetId, a => a);

            foreach (var pl in pointsLookup)
            {
                await this.geoFenceService.HandlePoints(pl.Key, pl.ToArray());
            }

            return Ok();
        }
    }
}
