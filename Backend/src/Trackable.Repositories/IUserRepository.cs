using Trackable.Models;
using System;
using System.Threading.Tasks;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the user repository.
    /// </summary>
    public interface IUserRepository : IRepository<Guid, User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<bool> AnyAsync();
    }
}
