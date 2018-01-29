using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the token repository.
    /// </summary>
    public interface ITokenRepository : IRepository<Guid, JwtToken>
    {
        Task<IEnumerable<JwtToken>> GetActiveByUserAsync(User user);

        Task<IEnumerable<JwtToken>> GetActiveByDeviceAsync(TrackingDevice device);

        Task DisableTokensByUserAsync(User user);

        Task DisableTokensByDeviceAsync(TrackingDevice device);
    }
}
