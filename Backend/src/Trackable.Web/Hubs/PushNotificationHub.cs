using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using Trackable.Web.Auth;
using Trackable.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Trackable.Common;
using Trackable.Models;
using System.Collections.Generic;

namespace Trackable.Web
{
    /// <summary>
    /// SignalR Service Hub
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer", Policy = UserRoles.TrackingDevice)]
    public class PushNotificationHub : Hub
    {
        private readonly ILogger logger;
        private readonly IDispatchingService dispatchingService;

        public PushNotificationHub(IDispatchingService dispatchingService, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<PushNotificationHub>();
            this.dispatchingService = dispatchingService;
        }

        public override async Task OnConnectedAsync()
        {
            var principal = Context.User as ClaimsPrincipal;

            var dispatches = await this.dispatchingService.GetByDeviceIdAsync(ClaimsReader.ReadDeviceId(principal));

            foreach (var dispatch in dispatches.ToArray())
            {
                Clients.Client(this.Context.ConnectionId).SendAsync("DispatchParameters", dispatch);
            }

            if (principal != null)
            {
                var deviceId = ClaimsReader.ReadDeviceId(principal);

                if (!string.IsNullOrEmpty(deviceId))
                {
                    dispatchingService.RegisterDeviceConnection(deviceId, this.Context.ConnectionId);
                }
            }
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var principal = Context.User as ClaimsPrincipal;

            if (principal != null)
            {
                var deviceId = ClaimsReader.ReadDeviceId(principal);

                if (!string.IsNullOrEmpty(deviceId))
                {
                    this.dispatchingService.DeleteDeviceConnection(deviceId);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
        
    }
}