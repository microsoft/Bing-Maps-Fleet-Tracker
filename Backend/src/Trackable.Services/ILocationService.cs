using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ILocationService : ICrudService<int, Location>
    {
        Task<IDictionary<string, int>> GetCountByAssetAsync(int locationid); 
    }
}
