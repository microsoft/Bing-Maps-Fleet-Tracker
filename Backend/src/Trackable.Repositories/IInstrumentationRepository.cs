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
