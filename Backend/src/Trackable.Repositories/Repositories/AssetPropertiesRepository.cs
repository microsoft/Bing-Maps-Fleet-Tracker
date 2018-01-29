using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    class AssetPropertiesRepository : DbRepositoryBase<int, AssetPropertiesData, AssetProperties>, IAssetPropertiesRepository
    {
        public AssetPropertiesRepository(
            TrackableDbContext db,
            IMapper mapper)
            : base(db, mapper)
        {
        }

        public override async Task<AssetProperties> AddAsync(AssetProperties assetProperties)
        {
            await checkValidity(assetProperties);

            return await base.AddAsync(assetProperties);
        }

        public Task checkValidity(AssetProperties assetProperties)
        {
            if (assetProperties.AssetHeight < 0 ||
                assetProperties.AssetWidth < 0 || 
                assetProperties.AssetLength < 0 ||
                assetProperties.AssetWeight < 0 ||
                assetProperties.AssetAxels < 0 || assetProperties.AssetAxels > 20 ||
                assetProperties.AssetMaxGradient < 0 ||
                assetProperties.AssetMinTurnRadius < 0 ||
                assetProperties.AssetTrailers < 0 || assetProperties.AssetTrailers > 10)
            {
                throw new BadArgumentException("One of the Asset Properties are invalid");
            }

            return Task.FromResult(0);
        }


        protected override Expression<Func<AssetPropertiesData, object>>[] Includes => null;

    }


}
