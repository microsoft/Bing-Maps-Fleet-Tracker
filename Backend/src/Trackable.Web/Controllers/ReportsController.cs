using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trackable.Common;
using Microsoft.Extensions.Logging;
using Trackable.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet("")]
        public async Task<IEnumerable<Metric>> Get()
        {
            return await this.reportingService.GetAssetsMetricsAsync();
        }
    }
}
