using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IAssetService : ICrudService<string, Asset>
    {
        Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions();
    }

}
