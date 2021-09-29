// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;

namespace Trackable.Web.Controllers
{
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService settingsService;
        private readonly IInstrumentationService instrumentationService;

        public SettingsController(
            IInstrumentationService instrumentationService,
            ISettingsService settingsService,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.instrumentationService = instrumentationService.ThrowIfNull(nameof(instrumentationService));
            this.settingsService = settingsService.ThrowIfNull(nameof(settingsService));
        }

        /// <summary>
        /// Get information about BMFT version
        /// </summary>
        /// <returns>Version information</returns>
        [HttpGet("version")]
        [Authorize(UserRoles.Owner)]
        public async Task<UpdateStatus> GetVersionInfo()
        {
            return await this.settingsService.GetUpdateStatus();
        }

        /// <summary>
        /// Get subscription keys
        /// </summary>
        /// <returns>List of subscription keys</returns>
        [HttpGet("subscriptionkeys")]
        [Authorize(UserRoles.TrackingDevice)]
        public IEnumerable<SubscriptionKey> GetSubscriptionKeys()
        {
            return this.settingsService.GetSubscriptionKeys();
        }

        /// <summary>
        /// Get status of instrumentation approval
        /// </summary>
        /// <returns>Null if not prompted previously, boolean otherwise</returns>
        [HttpGet("instrumentation")]
        [Authorize(UserRoles.Owner)]
        public async Task<bool?> GetInstrumentationApproval()
        {
            return await this.instrumentationService.GetInstrumentationApproval();
        }

        /// <summary>
        /// Update status of the instrumentation approval
        /// </summary>
        /// <param name="approval">Approval status</param>
        /// <returns>Ok response</returns>
        [HttpPost("instrumentation")]
        [Authorize(UserRoles.Owner)]
        public async Task SetInstrumentationApproval(bool approval)
        {
            await this.instrumentationService.SetInstrumentationApproval(approval);
        }
    }
}
