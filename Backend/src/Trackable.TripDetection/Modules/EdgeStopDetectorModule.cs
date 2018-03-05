// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.TripDetection.Exceptions;
using Trackable.Configurations;
using Trackable.Models;
using Trackable.TripDetection.Helpers;
using Trackable.Common;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that injects stops at the beginning of every stream of points to over come
    /// the cold start, and at the end of the stream if we have not recieved any new points in
    /// a while.
    /// </summary>
    class EdgeStopDetectorModule : StopDetectorBase
    {
        /// <summary>
        /// Number of seconds from last reading before stops are injected
        /// </summary>
        private readonly int minimumSecondsBeforeTailInjection;

        /// <summary>
        /// Component that injects fake stops at the beginning and end of the stream of points
        /// </summary>
        /// <param name="minimumSecondsBeforeTailInjection">Minimum number of seconds from last reading before stops are injected</param>
        public EdgeStopDetectorModule(
            [Configurable(129600)] int minimumSecondsBeforeTailInjection)
        {
            if (minimumSecondsBeforeTailInjection < 0)
            {
                throw new ModuleConfigurationException("Number of seconds before injection must not be negative");
            }

            this.minimumSecondsBeforeTailInjection = minimumSecondsBeforeTailInjection;
        }

        protected async override Task<TripDetectionContext> ProcessInternal(TripDetectionContext input)
        {
            var filteredPoints = input.FilteredOrderedPoints;

            if (!filteredPoints.Any())
            {
                return input;
            }

            // Inject a stop segment at the beginning
            input.TripSegments.Insert(0, new StoppedSegment(GenerateFakePoints(filteredPoints.First())));

            var timeSinceLastReading =
                DateTimeUtils.DifferenceInMilliseconds(
                    filteredPoints.Last().DeviceTimestampUtc,
                    DateTimeUtils.CurrentTimeInMillseconds()) / 1000;

            // If time since the last reading exceeds the threshold, add a stopped
            // segment at the end of the stream
            if (timeSinceLastReading > this.minimumSecondsBeforeTailInjection)
            {
                input.TripSegments.Add(new StoppedSegment(GenerateFakePoints(filteredPoints.Last())));
            }

            return input;
        }

        // Generate two fake points that are Int32.Max time apart
        private IList<TrackingPoint> GenerateFakePoints(TrackingPoint seedPoint)
        {
            var firstPoint = (TrackingPoint) seedPoint.Clone() ;
            firstPoint.DeviceTimestampUtc = 0;

            var lastPoint = (TrackingPoint) seedPoint.Clone();
            lastPoint.DeviceTimestampUtc = int.MaxValue;

            return new List<TrackingPoint>
                {
                    firstPoint,
                    lastPoint
                };
        }

    }
}
