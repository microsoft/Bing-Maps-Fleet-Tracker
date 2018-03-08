// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IUserService : ICrudService<string, User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetOrCreateUserByEmailAsync(string email, string name, string claimsId);

        Task<Role> GetRoleAsync(string role);
    }
}
