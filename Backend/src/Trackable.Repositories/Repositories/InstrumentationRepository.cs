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
    class InstrumentationRepository : DbRepositoryBase<Guid, DeploymentIdData, DeploymentId>, IInstrumentationRepository
    {
        public InstrumentationRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }


        protected override Expression<Func<DeploymentIdData, object>>[] Includes => null;

        public async Task<Guid> GetDeploymentIdAsync()
        {
            return (await this.Db.DeploymentId.SingleAsync()).Id;
        }
    }
}
