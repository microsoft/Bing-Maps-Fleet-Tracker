namespace Trackable.Web.DTOs
{
    public class TrackingPointDto
    {
        public long Time { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Provider { get; set; }

        public int LocationProvider { get; set; }

        public double? Accuracy { get; set; }

        public double? Speed { get; set; }

        public double? Altitude { get; set; }

        public double? Bearing { get; set; }

        public string AssetId { get; set; }

        public string TrackingDeviceId { get; set; }
    }
}
