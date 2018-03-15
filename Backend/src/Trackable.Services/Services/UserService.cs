// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class UserService : CrudServiceBase<string, User, IUserRepository>, IUserService
    {
        private readonly IRoleRepository roleRepository;

        internal static string OwnerEmail { get; set; }

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
            : base(userRepository)
        {
            this.roleRepository = roleRepository;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await this.repository.GetUserByEmailAsync(email);
        }

        public async Task<Role> GetRoleAsync(string role)
        {
            return await this.roleRepository.GetRoleAsync(role);
        }

        public async Task<User> GetOrCreateUserByEmailAsync(string email, string name, string claimsId)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                var signupRole = await (email == OwnerEmail
                    ? roleRepository.GetRoleAsync(UserRoles.Owner)
                    : roleRepository.GetRoleAsync(UserRoles.Pending));

                user = await this.AddAsync(new User
                {
                    Name = name ?? email,
                    Email = email,
                    ClaimsId = claimsId ?? "",
                    Role = signupRole
                });
            }

            return user;
        }
    }
}
