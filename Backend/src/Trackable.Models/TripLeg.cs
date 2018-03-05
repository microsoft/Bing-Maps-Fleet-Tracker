// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Trackable.Models.Helpers;

namespace Trackable.Models
{
    [Serializable]
    public class TripLeg : ModelBase<int>
    {
        /// <summary>
        /// The Start Location ID.
        /// </summary>
        [Mutable]
        public int StartLocationId { get; set; }

        /// <summary>
        /// The End Location ID.
        /// </summary>
        [Mutable]
        public int EndLocationId { get; set; }

        public Location EndLocation { get; set; }

        public Location StartLocation { get; set; }

        /// <summary>
        /// The Trip ID.
        /// </summary>
        [Mutable]
        public int TripId { get; set; }

        /// <summary>
        /// The average speed during the trip in m/s.
        /// </summary>
        [Mutable]
        public double AverageSpeed { get; set; }

        /// <summary>
        /// The start time of the trip leg in UTC.
        /// </summary>
        [Mutable]
        public DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// The end time of the trip leg in UTC.
        /// </summary>
        [Mutable]
        public DateTime EndTimeUtc { get; set; }

        /// <summary>
        /// The points of the trip leg that correspond to the trip route.
        /// </summary>
        [Mutable]
        public IEnumerable<IPoint> Route { get; set; }
    }
}
