// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Linq.Expressions;
using Trackable.EntityFramework;
using Trackable.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Trackable.Repositories
{
     class DispatchingRepository : DbRepositoryBase<int, DispatchData, Dispatch>, IDispatchingRepository
    {
        public DispatchingRepository(
        TrackableDbContext db,
        IMapper mapper)
        : base(db, mapper)
        {
        }

        public override async Task<Dispatch> AddAsync(Dispatch model)
        {
            return await base.AddAsync(model);
        }

        public async Task<IEnumerable<Dispatch>> GetByDeviceIdAsync(string deviceId)
        {
            var data = await this
                .FindBy(a => a.DeviceId == deviceId)
                .ToListAsync();
            return data.Select(d => this.ObjectMapper.Map<Dispatch>(d));
        }

        protected override Expression<Func<DispatchData, object>>[] Includes => new Expression<Func<DispatchData, object>>[]
        {
            data => data.Points
        };
    }
}
