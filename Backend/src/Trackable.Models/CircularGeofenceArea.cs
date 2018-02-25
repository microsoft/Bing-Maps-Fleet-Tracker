namespace Trackable.Models
{
    public class CircularGeoFenceArea : IGeoFenceArea
    {
        public Point Center { get; set; }

        public double RadiusInMeters { get; set; }

        public GeoFenceAreaType AreaType => GeoFenceAreaType.Circular;
    }
}
