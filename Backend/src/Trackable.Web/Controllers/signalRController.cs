using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trackable.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using AutoMapper;
using Trackable.Models;
using Trackable.Web;
using Trackable.Web.Dtos;

namespace Trackable.Web.Controllers
{

    /// <summary>
    /// controls push notification
    /// </summary>
    [Route("api/sendPushNotification")]
    public class SignalRController
    {
        private readonly IHubContext<PushNotificationHub> hubContext;
        private readonly IDispatchingService dispatchingService;
        private readonly IAssetService assetService;
        private readonly ILogger logger;
        private readonly IMapper dtoMapper;


        public SignalRController(
            IDispatchingService dispatchingService,
            IAssetService assetService,
            IHubContext<PushNotificationHub> hubContext,
            ILoggerFactory loggerFactory,
            IMapper dtoMapper)
        {
            this.hubContext = hubContext;
            this.dispatchingService = dispatchingService;
            this.dtoMapper = dtoMapper;
            this.assetService = assetService;
            this.logger = loggerFactory.CreateLogger<SignalRController>();
        }

        /// <summary>
        /// Adds new dispatch to database
        /// Send dispatch to Mobile Client
        /// </summary>
        /// <param name="dispatchingParameters">The parameters required for dispatching</param>
        /// <returns>Boolean for operation success</returns>
        [HttpPost]
        public async Task<DeviceState> Post([FromBody]DispatchDto dispatchingParameters)
        {
            if (dispatchingParameters.DeviceId == null)
                return DeviceState.Invalid;
            
            var connectionId = this.dispatchingService.GetDeviceConnection(dispatchingParameters.DeviceId);

            var dispatchModel = this.dtoMapper.Map<Dispatch>(dispatchingParameters);
            var savedModel = await this.dispatchingService.AddAsync(dispatchModel);

            if (connectionId == null)
                return DeviceState.Offline;

            savedModel.DateTime = dispatchModel.DateTime;
            await hubContext.Clients.Client(connectionId).SendAsync("DispatchParameters", savedModel);

            return DeviceState.Online;
        }
    }

    public enum DeviceState
    {
        Online,
        Offline,
        Invalid,
    }
}