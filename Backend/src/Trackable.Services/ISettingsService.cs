using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ISettingsService
    {
        Task<UpdateStatus> GetUpdateStatus();

        IEnumerable<SubscriptionKey> GetSubscriptionKeys();
    }
}
