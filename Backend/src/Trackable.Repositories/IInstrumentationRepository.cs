// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface IInstrumentationRepository: IRepository<Guid, DeploymentId>
    {
        Task<Guid> GetDeploymentIdAsync();
    }
}
