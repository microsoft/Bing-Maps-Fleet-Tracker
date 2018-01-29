using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Services
{
    public class Metric
    {
        public string Name { get; }

        // Todo: Make Enum
        public string Units { get; }

        // Todo: Make Enum
        public string AggregatedBy { get; }

        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public Metric(string name, string units, string aggregatedBy)
        {
            this.Name = name;
            this.Units = units;
            this.AggregatedBy = aggregatedBy;
        }
    }
}
