// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Trackable.TripDetection.Exceptions;
using Trackable.Configurations;
using Trackable.TripDetection.Helpers;
using System;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that segments a stream of labeled points into trips, based on the
    /// time spent in each stop.
    /// </summary>
    class TimeBasedTripExtractorModule : TripExtractorBase
    {
        /// <summary>
        /// Minimum stop time for a point in seconds to mark the end of a trip
        /// </summary>
        private readonly int miniumumTripDwellTime;

        // <summary>
        // Component that segments stream of points into trips
        // </summary>
        // <param name="miniumumTripDwellTime">Minimum stop time in seconds for a stop to qualify as end of trip</param>
        /// <summary>
        /// Component that extracts trips from candidate trip legs.
        /// </summary>
        /// <param name="miniumumTripDwellTime">Minimum stop time in seconds for a stop to qualify as end of trip</param>
        /// <param name="miniumumTripPoints">Minimum number of points that a trip must have</param>
        /// <param name="minimumTripDistance">Minimum distance covered by a trip</param>
        public TimeBasedTripExtractorModule(
            [Configurable(20 * 60)] int miniumumTripDwellTime,
            [Configurable(5)] int miniumumTripPoints, 
            [Configurable(3000)] double minimumTripDistance)
            :base(miniumumTripPoints, minimumTripDistance)
        {
            if (miniumumTripDwellTime < 0)
            {
                throw new ModuleConfigurationException("Minimum trip dwell time must not be negative");
            }

            this.miniumumTripDwellTime = miniumumTripDwellTime;
        }

        protected override Predicate<StoppedSegment> isEndOfTrip => stop =>
        {
            return stop.GetDurationInSeconds() >= this.miniumumTripDwellTime;
        };
    }
}
