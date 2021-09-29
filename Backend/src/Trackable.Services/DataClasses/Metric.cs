// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

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
