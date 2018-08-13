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
        public async Task<Boolean> Post([FromBody]DispatchDto dispatchingParameters)
        {
            if (dispatchingParameters.DeviceId == null)
                return false;
            
            var connectionId = this.dispatchingService.GetDeviceConnection(dispatchingParameters.DeviceId);
            if (connectionId == null)
                return false;

            var dispatchModel = this.dtoMapper.Map<Dispatch>(dispatchingParameters);
            var savedModel = await this.dispatchingService.AddAsync(dispatchModel);

            savedModel.DateTime = dispatchModel.DateTime;
            await hubContext.Clients.Client(connectionId).SendAsync("DispatchParameters", savedModel);

            return true;
        }
    }
}