// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IDispatchingService
    {
        Task<IEnumerable<DispatchingResults>> CallRoutingAPI(DispatchingParameters dispatchingParameters, AssetProperties assetproperties);
        
        string GenerateURL(DispatchingParameters dispatchingParameters, AssetProperties assetProperties, bool GetAlternativeCarRoute);

    }
}
