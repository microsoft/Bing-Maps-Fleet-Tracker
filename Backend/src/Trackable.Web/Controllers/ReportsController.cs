// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Services;

namespace Trackable.Web.Controllers
{
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportingService reportingService;

        public ReportsController(
            IReportingService reportingService,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.reportingService = reportingService.ThrowIfNull(nameof(reportingService));
        }

        /// <summary>
        /// Get report metrics
        /// </summary>
        /// <returns>List of metrics</returns>
        [HttpGet("")]
        public async Task<IEnumerable<Metric>> Get()
        {
            return await this.reportingService.GetAssetsMetricsAsync();
        }
    }
}
