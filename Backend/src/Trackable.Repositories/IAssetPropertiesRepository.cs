using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface IAssetPropertiesRepository: IRepository<int, AssetProperties>
    {
        Task checkValidity(AssetProperties assetProperties);
    }
}
