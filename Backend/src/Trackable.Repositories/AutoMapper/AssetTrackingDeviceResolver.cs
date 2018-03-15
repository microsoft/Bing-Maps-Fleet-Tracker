// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System.Linq;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories.AutoMapper
{
    class AssetTrackingDeviceResolver : IValueResolver<Asset, AssetData, TrackingDeviceData>
    {
        private readonly TrackableDbContext db;

        public AssetTrackingDeviceResolver(TrackableDbContext db)
        {
            this.db = db;
        }
        
        public TrackingDeviceData Resolve(Asset source, AssetData destination, TrackingDeviceData destMember, ResolutionContext context)
        {
            return this.db.TrackingDevices.SingleOrDefault(a => !a.Deleted && a.Id == source.TrackingDeviceId);
        }
    }
}
