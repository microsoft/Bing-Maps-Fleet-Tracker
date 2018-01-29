using AutoMapper;
using System.Linq;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories.AutoMapper
{
    class TokenTrackingDeviceResolver : IValueResolver<JwtToken, TokenData, TrackingDeviceData>
    {
        private readonly TrackableDbContext db;

        public TokenTrackingDeviceResolver(TrackableDbContext db)
        {
            this.db = db;
        }

        public TrackingDeviceData Resolve(JwtToken source, TokenData destination, TrackingDeviceData destMember, ResolutionContext context)
        {
            return this.db.TrackingDevices.SingleOrDefault(a => !a.Deleted && a.Id == source.TrackingDeviceId);
        }
    }
}
