namespace Trackable.Models
{
    public interface IGeoFenceArea
    {
        /// <summary>
        /// The way the geofence specifies its area
        /// </summary>
        GeoFenceAreaType AreaType { get; }
    }

    /// <summary>
    /// The way the geofence area is specified
    /// </summary>
    public enum GeoFenceAreaType
    {
        Unknown = 0,
        Polygon = 1,
        Circular = 2
    }
}
