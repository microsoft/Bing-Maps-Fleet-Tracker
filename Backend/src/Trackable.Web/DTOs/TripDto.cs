// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Web.Dtos
{
    public class TripDto
    {
        /// <summary>
        /// Autogenerated Trip Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The location which the trip started from
        /// </summary>
        public LocationDto StartLocation { get; set; }

        /// <summary>
        /// The location which the trip ended at
        /// </summary>
        public LocationDto EndLocation { get; set; }

        /// <summary>
        /// The id of the asset that performed this trip
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The id of the tracking device that performed this trip
        /// </summary>
        public string TrackingDeviceId { get; set; }

        /// <summary>
        /// The legs (segments) of the trip
        /// </summary>
        public IEnumerable<TripLegDto> TripLegs { get; set; }

        /// <summary>
        /// Trip start time in UTC
        /// </summary>
        public DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// Trip end time in UTC
        /// </summary>
        public DateTime EndTimeUtc { get; set; }

        /// <summary>
        /// Trip duration in minutes
        /// </summary>
        public double DurationInMinutes { get; set; }
    }
}
