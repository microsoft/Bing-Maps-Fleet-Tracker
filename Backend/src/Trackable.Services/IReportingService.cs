// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trackable.Services
{
    public interface IReportingService
    {
        Task<IEnumerable<Metric>> GetAssetsMetricsAsync();
    }
}
