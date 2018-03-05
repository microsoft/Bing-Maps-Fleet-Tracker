// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.Web.Auth;
using Trackable.Web.Dtos;

namespace Trackable.Web.Controllers
{
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly ITrackingDeviceService deviceService;
        private readonly ITrackingPointService pointService;
        private readonly IGeoFenceService geoFenceService;
        private readonly IHubContext<DynamicHub> deviceAdditionHubContext;
        private readonly ITokenService tokenService;
        private readonly IMapper dtoMapper;

        public DevicesController(
            ITrackingDeviceService deviceService,
            ITrackingPointService pointService,
            IGeoFenceService geoFenceService,
            ILoggerFactory loggerFactory,
            IHubContext<DynamicHub> deviceAdditionHubContext,
            ITokenService tokenService,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.deviceService = deviceService;
            this.pointService = pointService;
            this.geoFenceService = geoFenceService;
            this.deviceAdditionHubContext = deviceAdditionHubContext;
            this.tokenService = tokenService;
            this.dtoMapper = dtoMapper;
        }

        // GET api/devices
        [HttpGet]
        public async Task<IEnumerable<TrackingDeviceDto>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                var results = await this.deviceService.ListAsync();
                return this.dtoMapper.Map<IEnumerable<TrackingDeviceDto>>(results);
            }

            IEnumerable<TrackingDevice> taggedResults = null;
            if (!string.IsNullOrEmpty(tags))
            {
                var tagsArray = tags.Split(',');
                if (includesAllTags)
                {
                    taggedResults = await this.deviceService.FindContainingAllTagsAsync(tagsArray);
                }
                else
                {
                    taggedResults = await this.deviceService.FindContainingAnyTagsAsync(tagsArray);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return this.dtoMapper.Map<IEnumerable<TrackingDeviceDto>>(taggedResults);
            }

            var resultsByName = await this.deviceService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return this.dtoMapper.Map<IEnumerable<TrackingDeviceDto>>(resultsByName);
            }

            return this.dtoMapper.Map<IEnumerable<TrackingDeviceDto>>(
                taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id)));
        }

        // GET api/devices/5
        [HttpGet("{id}")]
        public async Task<TrackingDeviceDto> Get(string id)
        {
            var result = await this.deviceService.GetAsync(id);

            return this.dtoMapper.Map<TrackingDeviceDto>(result);
        }

        // Post api/devices/5/token
        [HttpPost("{id}/token")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> GetDeviceToken(string id)
        {
            var device = await this.deviceService.GetAsync(id);

            return Json(await this.tokenService.GetLongLivedDeviceToken(device, false));
        }

        // GET api/devices/all/positions
        [HttpGet("all/positions")]
        public async Task<IDictionary<string, TrackingPointDto>> GetLatestPositions(string id)
        {
            var results = await this.deviceService.GetDevicesLatestPositions();

            return this.dtoMapper.Map<IDictionary<string, TrackingPointDto>>(results);
        }

        // GET api/devices/5/points
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPointDto>> GetPoints(string id)
        {
            var results = await this.pointService.GetByDeviceIdAsync(id);

            return this.dtoMapper.Map<IEnumerable<TrackingPointDto>>(results);
        }

        // POST api/devices
        [HttpPost]
        [Authorize(UserRoles.DeviceRegistration)]
        public async Task<IActionResult> Post([FromBody]TrackingDeviceDto device, [FromQuery]string nonce = null)
        {
            var model = this.dtoMapper.Map<TrackingDevice>(device);

            var response = await this.deviceService.AddOrUpdateDeviceAsync(model);

            await this.deviceAdditionHubContext.Clients.All.InvokeAsync("DeviceAdded", nonce);

            return Json(await this.tokenService.GetLongLivedDeviceToken(response, false));
        }

        // POST api/devices
        [HttpPost("batch")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> Post([FromBody]TrackingDeviceDto[] devices)
        {
            var models = this.dtoMapper.Map<TrackingDevice[]>(devices);

            var addedDevices = await this.deviceService.AddAsync(models);

            var tokens = new List<string>();
            foreach (var device in addedDevices)
            {
                tokens.Add(await this.tokenService.GetLongLivedDeviceToken(device, false));
            }

            return Json(tokens);
        }

        // POST api/devices/5/points
        [HttpPost("{id}/points")]
        [Authorize(UserRoles.TrackingDevice)]
        public async Task<IActionResult> PostPointsToDevice(string id, [FromBody]TrackingPointDto[] points)
        {
            var models = this.dtoMapper.Map<TrackingPoint[]>(points);

            var subject = ClaimsReader.ReadSubject(this.User);
            var audience = ClaimsReader.ReadAudience(this.User);

            if (audience == JwtAuthConstants.DeviceAudience && id != subject)
            {
                return Forbid();
            }

            models.ForEach((point) => point.TrackingDeviceId = id);
            var addedPoints = await this.pointService.AddAsync(models);
            await this.geoFenceService.HandlePoints(addedPoints.First().AssetId, addedPoints.ToArray());

            return Ok();
        }

        // PUT api/devices/5
        [HttpPut("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<TrackingDeviceDto> Put(string id, [FromBody]TrackingDeviceDto device)
        {
            var model = this.dtoMapper.Map<TrackingDevice>(device);

            var result = await this.deviceService.UpdateAsync(id, model);

            return this.dtoMapper.Map<TrackingDeviceDto>(result);
        }

        // DELETE api/devices/5
        [HttpDelete("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task Delete(string id)
        {
            var device = await this.deviceService.GetAsync(id);

            if (device != null)
            {
                await this.tokenService.DisableDeviceTokens(device);
                await this.deviceService.DeleteAsync(device.Id);
            }
        }

        // GET api/devices/qrcode
        [HttpGet("qrcode")]
        [Authorize(UserRoles.Administrator)]
        public IActionResult GetProvisioningQrCode(string nonce = null, int height = 300, int width = 300, int margin = 0)
        {
            string queryParams = string.Empty;
            if (!String.IsNullOrEmpty(nonce))
            {
                queryParams = $"?nonce={nonce}";
            }

            var data = new PhoneClientData
            {
                BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}",
                RegistrationUrl = $"{this.Request.Scheme}://{this.Request.Host}/api/devices{queryParams}",
                RegistrationToken = this.tokenService.GetShortLivedDeviceRegistrationToken()
            };

            return File(
                this.deviceService.GetDeviceProvisioningQrCode(
                    data,
                    height,
                    width,
                    margin),
                "image/png");
        }
    }
}
