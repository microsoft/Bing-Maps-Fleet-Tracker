// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    class TrackingDeviceRepository : DbRepositoryBase<string, TrackingDeviceData, TrackingDevice>, ITrackingDeviceRepository
    {
        public TrackingDeviceRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions()
        {
            return await this.Db.TrackingDevices
                .AsNoTracking()
                .Include(d => d.LatestPosition)
                .Where(d => !d.Deleted && d.LatestPosition != null)
                .ToDictionaryAsync(d => d.Name, d => this.ObjectMapper.Map<TrackingPoint>(d.LatestPosition));
        }

        protected override Expression<Func<TrackingDeviceData, object>>[] Includes => new Expression<Func<TrackingDeviceData, object>>[]
        {
            data => data.Asset,
            data => data.Tags
        };
    }
}
