using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the asset repository.
    /// </summary>
    public interface IAssetRepository : 
        IRepository<string, Asset>,
        IDbCountableRepository<string, AssetData, Asset>,
        IDbTaggedRepository<string, AssetData, Asset>
    {
        Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions();

        Task<int> GetNumberOfActiveAssets(DateTime activeThreshold);
    }
}
