// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITrackingPointService : ICrudService<int, TrackingPoint>
    {
        Task<IEnumerable<TrackingPoint>> GetByAssetIdAsync(string assetId);
        Task<IEnumerable<TrackingPoint>> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<TrackingPoint>> GetNearestPoints(int tripId, IPoint point, int count);
    }
}
