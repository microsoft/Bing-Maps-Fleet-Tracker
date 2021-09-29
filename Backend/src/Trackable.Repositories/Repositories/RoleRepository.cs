// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    class RoleRepository : DbRepositoryBase<Guid, RoleData, Role>, IRoleRepository
    {
        public RoleRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<Role> GetRoleAsync(string role)
        {
            var data = await this.FindBy(r => r.Name == role)
                .SingleOrDefaultAsync();
            return this.ObjectMapper.Map<Role>(data);
        }

        protected override Expression<Func<RoleData, object>>[] Includes => null;

    }
}
