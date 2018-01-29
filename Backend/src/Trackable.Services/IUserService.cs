using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IUserService : ICrudService<Guid, User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetOrCreateUserByEmailAsync(string email, string name, string claimsId);

        Task<Role> GetRoleAsync(string role);
    }
}
