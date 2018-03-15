// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories.AutoMapper
{
    class GeoFenceAssetResolver : IValueResolver<GeoFence, GeoFenceData, ICollection<AssetData>>
    {
        private readonly TrackableDbContext db;

        public GeoFenceAssetResolver(TrackableDbContext db)
        {
            this.db = db;
        }

        public ICollection<AssetData> Resolve(GeoFence source, GeoFenceData destination, ICollection<AssetData> destMember, ResolutionContext context)
        {
            return this.db.Assets.Where(a => !a.Deleted && source.AssetIds.Contains(a.Id)).ToList();
        }
    }
}
