// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("TrackingPoints")]
    public class TrackingPointData : EntityBase<int>
    {
        /// <summary>
        /// The device time in milliseconds, UTC.
        /// </summary>
        public long DeviceTimestampUtc { get; set; }

        /// <summary>
        /// The point location.
        /// </summary>
        public DbGeography Location { get; set; }

        /// <summary>
        /// The provider such as GPS, network, passive or fused
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The provider such as GPS, network, passive or fused
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
        public int? TripId { get; set; }

        /// <summary>
        /// The parent trip.
        /// </summary>
        public TripData Trip { get; set; }

        /// <summary>
        /// The asset ID.
        /// </summary>
        [Required]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset
        /// </summary>
        public AssetData Asset { get; set; }

        /// <summary>
        /// The tracking device ID.
        /// </summary>
        [Required]
        public string TrackingDeviceId { get; set; }

        // The tracking device.
        public TrackingDeviceData TrackingDevice { get; set; }
    }
}