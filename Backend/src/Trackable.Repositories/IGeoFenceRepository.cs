// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the Geofence repository.
    /// </summary>
    public interface IGeoFenceRepository : 
        IRepository<string, GeoFence>,
        IDbCountableRepository<string, GeoFenceData, GeoFence>,
        IDbNamedRepository<string, GeoFenceData, GeoFence>,
        IDbTaggedRepository<string, GeoFenceData, GeoFence>
    {
        Task<Dictionary<GeoFence, bool>> GetByAssetIdWithIntersectionAsync(string assetId, IPoint[] points);
    }
}
