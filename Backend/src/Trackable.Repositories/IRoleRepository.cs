// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Trackable.EntityFramework;
using Trackable.Models;
using System;
using System.Threading.Tasks;
using Trackable.Common;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the asset repository.
    /// </summary>
    public interface IRoleRepository : IRepository<Guid, Role>
    {
        Task<Role> GetRoleAsync(string role);
    }
}
