using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Services
{
    public interface IReportingService
    {
        Task<IEnumerable<Metric>> GetAssetsMetricsAsync();
    }
}
