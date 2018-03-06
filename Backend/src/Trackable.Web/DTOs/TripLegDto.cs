// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Web.Dtos
{
    public class TripLegDto
    {
        /// <summary>
        /// The starting location of the trip leg
        /// </summary>
        public LocationDto EndLocation { get; set; }

        /// <summary>
        /// The ending location of the trip leg
        /// </summary>
        public LocationDto StartLocation { get; set; }

        /// <summary>
        /// The average speed in km/h
        /// </summary>
        public double AverageSpeed { get; set; }

        /// <summary>
        /// The trip leg starting time in UTC
        /// </summary>
        public DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// The trip leg ending time in UTC
        /// </summary>
        public DateTime EndTimeUtc { get; set; }

        /// <summary>
        /// The route of the trip leg
        /// </summary>
        public IEnumerable<Point> Route { get; set; }
    }
}
