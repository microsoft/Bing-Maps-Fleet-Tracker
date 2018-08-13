// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IDispatchingService
    {
        Task<IEnumerable<DispatchingResults>> CallRoutingAPI(Dispatch dispatchingParameters, AssetProperties assetproperties);
        
        string GenerateURL(Dispatch dispatchingParameters, AssetProperties assetProperties, bool GetAlternativeCarRoute);

        void RegisterDeviceConnection(string deviceId, string connectionId);

        string GetDeviceConnection(string deviceId);

        void DeleteDeviceConnection(string deviceId);

        Task<Dispatch> AddAsync(Dispatch dispatch);

        Task<IEnumerable<Dispatch>> GetByDeviceIdAsync(string deviceId);

    }
}
