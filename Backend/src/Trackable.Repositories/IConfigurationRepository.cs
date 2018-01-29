using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the configuration repository.
    /// </summary>
    public interface IConfigurationRepository : IRepository<string, string, Configuration>
    {
    }
}
