// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace Trackable.Web.Dtos
{
    public class TrackingPointDto
    {
        /// <summary>
        /// Unix time stamp of the time of the point's collection, as reported by the device
        /// </summary>
        [Required]
        public long Time { get; set; }

        /// <summary>
        /// Tracking Point Longitude
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Tracking Point Latitude
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// The location provider used to acquire this point, eg. Network, GPS
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The location provider number
        /// </summary>
        public int LocationProvider { get; set; }

        /// <summary>
        /// The estimated accuracy, in meters, of the location as reported by the location provider
        /// </summary>
        public double? Accuracy { get; set; }

        /// <summary>
        /// The instantenous speed when the point was recorded
        /// </summary>
        public double? Speed { get; set; }

        /// <summary>
        /// The altitude when the point was recorded
        /// </summary>
        public double? Altitude { get; set; }
        
        /// <summary>
        /// The bearing of the device when the point was recorded
        /// </summary>
        public double? Bearing { get; set; }

        /// <summary>
        /// The id of the asset this point is tied to
        /// </summary>
        public string AssetId { get; set; }
        
        /// <summary>
        /// The id of the device this point is tied to
        /// </summary>
        [Required]
        public string TrackingDeviceId { get; set; }
    }
}
