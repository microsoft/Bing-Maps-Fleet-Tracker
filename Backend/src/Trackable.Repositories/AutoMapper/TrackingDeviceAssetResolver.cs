using AutoMapper;
using System.Linq;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories.AutoMapper
{
    class TrackingDeviceAssetResolver : IValueResolver<TrackingDevice, TrackingDeviceData, AssetData>
    {
        private readonly TrackableDbContext db;

        public TrackingDeviceAssetResolver(TrackableDbContext db)
        {
            this.db = db;
        }

        public AssetData Resolve(TrackingDevice source, TrackingDeviceData destination, AssetData destMember, ResolutionContext context)
        {
            return this.db.Assets.SingleOrDefault(a => !a.Deleted && a.Id == source.AssetId);
        }
    }
}
