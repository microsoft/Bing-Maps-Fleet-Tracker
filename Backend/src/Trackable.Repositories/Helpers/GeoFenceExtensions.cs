using Trackable.Models;

namespace Trackable.Repositories.Helpers
{
    public static class GeoFenceExtensions
    {
        public static bool IsTriggeredByPoint(this GeoFence geofence, IPoint latestPoint)
        {
            var intersects = GeographyHelper.CreatePolygon(geofence.FencePolygon).MakeValid().Intersects(GeographyHelper.CreateDbPoint(latestPoint));

            if (geofence.FenceType == FenceType.Inbound)
            {
                return intersects;
            }
            else
            {
                return !intersects;
            }
        }
    }
}
