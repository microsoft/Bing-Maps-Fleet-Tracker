using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Configurations;
using Trackable.Models;
using Trackable.TripDetection.Exceptions;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that injects stops in case of a point blackout bigger than a thresholdDistance.
    /// </summary>
    internal class BlackoutStopDetectorModule : StopDetectorBase
    {
        /// <summary>
        /// Blackout distance before a stop is injected in meters
        /// </summary>
        private readonly double minimumBlackoutDistance;

        /// <summary>
        /// Blackout time before a stop is injected in seconds
        /// </summary>
        private readonly long minimumBlackoutTime;

        /// <summary>
        /// A component that injects stops when there is a distance larger than minimumBlackoutDistance between
        /// two consecutive points
        /// </summary>
        /// <param name="minimumBlackoutDistance">Minimum distance between two consecutive points before a stop is injected</param>
        /// <param name="minimumBlackoutTime">Minimum time between two consecutive points before a stop is injected</param>
        public BlackoutStopDetectorModule(
            [Configurable(10000)] double minimumBlackoutDistance,
            [Configurable(900)] long minimumBlackoutTime)
        {
            if (minimumBlackoutDistance <= 0)
            {
                throw new ModuleConfigurationException("Minimum blackout distance must be a positive real number");
            }

            if (minimumBlackoutTime <= 0)
            {
                throw new ModuleConfigurationException("Minimum blackout time must be a positive integer");
            };

            this.minimumBlackoutDistance = minimumBlackoutDistance;
            this.minimumBlackoutTime = minimumBlackoutTime;
        }

        protected async override Task<TripDetectionContext> ProcessInternal(TripDetectionContext input)
        {
            var oldSegments = input.TripSegments;
            var newSegments = new List<TripSegmentBase>();

            foreach (var segment in oldSegments)
            {
                if (!segment.IsMovingSegment)
                {
                    newSegments.Add(segment);
                    continue;
                }

                var lastIndex = 0;
                for (int i = 0; i < segment.Points.Count - 1; i++)
                {
                    var currentPoint = segment.Points[i];
                    var nextPoint = segment.Points[i + 1];

                    var distance = MathUtils.DistanceInMeters(currentPoint, nextPoint);
                    var timeDifferenceMs = DateTimeUtils.DifferenceInMilliseconds(currentPoint.DeviceTimestampUtc, nextPoint.DeviceTimestampUtc);

                    if (distance > this.minimumBlackoutDistance || timeDifferenceMs / 1000 > this.minimumBlackoutTime)
                    {
                        newSegments.Add(new MovingSegment(segment.Points.Skip(lastIndex).Take(i - lastIndex).ToList()));
                        newSegments.Add(new StoppedSegment(new List<TrackingPoint> { segment.Points[i], segment.Points[i + 1] }));
                        lastIndex = i + 1;
                    }
                }

                var remainingPointList = segment.Points.Skip(lastIndex).ToList();
                if (remainingPointList.Any())
                {
                    newSegments.Add(new MovingSegment(remainingPointList));
                }
            }

            input.TripSegments = newSegments;

            return input;
        }
    }
}
