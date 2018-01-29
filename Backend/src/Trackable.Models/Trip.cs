using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Trackable.Common;
using Trackable.Models.Helpers;

namespace Trackable.Models
{
    [Serializable]
    public class Trip : ModelBase<int>
    {
        /// <summary>
        /// The date time at which this trip was created
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// The trip's starting Location ID.
        /// </summary>
        [Mutable]
        public int StartLocationId { get; set; }

        /// <summary>
        /// The trip's ending Location ID.
        /// </summary>
        [Mutable]
        public int EndLocationId { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        /// <summary>
        /// The id of the asset that made this trip.
        /// </summary>
        [Mutable]
        public string AssetId { get; set; }

        /// <summary>
        /// The id of the tracking device that recorded this trip.
        /// </summary>
        [Mutable]
        public string TrackingDeviceId { get; set; }

        /// <summary>
        /// The starting time of the trip in UTC.
        /// </summary>
        [Mutable]
        public DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// The ending time of the trip in UTC.
        /// </summary>
        [Mutable]
        public DateTime EndTimeUtc { get; set; }

        /// <summary>
        /// The trip legs that constitute this trip.
        /// </summary>
        public IEnumerable<TripLeg> TripLegs { get; set; }

        /// <summary>
        /// Gets the duration of the trip in minutes
        /// </summary>
        public double DurationInMinutes => this.EndTimeUtc.Subtract(this.StartTimeUtc).TotalMinutes;

        /// <summary>
        /// Gets the start UTC time-stamp.
        /// </summary>
        public long StartTimeStampUtc => DateTimeUtils.ToUnixTime(StartTimeUtc);
    }
}
