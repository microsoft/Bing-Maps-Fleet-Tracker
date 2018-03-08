// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the asset repository.
    /// </summary>
    public interface IGeoFenceUpdateRepository : IRepository<int, GeoFenceUpdate>
    {
        Task<IDictionary<string, GeoFenceUpdate>> GetByGeofenceIdsAsync(string assetId, IEnumerable<string> geofenceIds);

        Task UpdateStatusAsync(int updateId, NotificationStatus status);
    }

}
