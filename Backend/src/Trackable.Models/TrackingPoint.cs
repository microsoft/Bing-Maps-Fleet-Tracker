using Newtonsoft.Json;
using Trackable.Models.Helpers;
using System;

namespace Trackable.Models
{
    [Serializable]
    public class TrackingPoint : ModelBase<int>, IPoint
    {
        /// <summary>
        /// Timestamp of creation of the point.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// The device time in milliseconds, UTC.
        /// </summary>
        public long DeviceTimestampUtc { get; set; }
        
        /// <summary>
        /// The provider such as GPS, network, passive or fused
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The location provider
        /// </summary>
        public int LocationProvider { get; set; }

        /// <summary>
        /// True if location recorded as part of debug
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Estimated accuracy of this location, in meters.
        /// </summary>
        public double? Accuracy { get; set; }

        /// <summary>
        /// Speed if it is available, in meters/second over ground.
        /// </summary>
        public double? Speed { get; set; }

        /// <summary>
        /// Altitude if available, in meters above the WGS 84 reference ellipsoid.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        /// Bearing, in degrees..
        /// </summary>
        public double? Bearing { get; set; }

        /// <summary>
        /// The parent trip ID.
        /// </summary>
        [Mutable]
        public int? TripId { get; set; }

        /// <summary>
        /// The asset ID.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The tracking device ID.
        /// </summary>
        public string TrackingDeviceId { get; set; }

        /// <summary>
        /// The latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude
        /// </summary>
        public double Longitude { get; set; }

        public object Clone()
        {
            return new TrackingPoint()
            {
                Accuracy = this.Accuracy,
                Altitude = this.Altitude,
                AssetId = this.AssetId,
                Bearing = this.Bearing,
                DeviceTimestampUtc = this.DeviceTimestampUtc,
                LocationProvider = this.LocationProvider,
                Provider = this.Provider,
                Speed = this.Speed,
                TrackingDeviceId = this.TrackingDeviceId,
                TripId = this.TripId,
                Latitude = this.Latitude,
                Longitude = this.Longitude
            };
        }
    }
}
