// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the user repository.
    /// </summary>
    public interface IUserRepository : IRepository<string, User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<bool> AnyAsync();
    }
}
