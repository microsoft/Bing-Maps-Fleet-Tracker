// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.TripDetection.Exceptions;
using Trackable.Configurations;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// Component that extracts clustered stops
    /// </summary>
    class ClusteredStopDetectorModule : StopDetectorBase
    {
        /// <summary>
        /// Maximum speed that can be classified as stopping speed in meters per second
        /// </summary>
        private readonly double maximumDwellSpeed;

        /// <summary>
        /// Minimum time during which speed is less than maximum dwell speed for stops
        /// </summary>
        private readonly int minimumDwellTime;

        /// <summary>
        /// The arc window size on which the angle is calculated
        /// </summary>
        private readonly int arcWindowSize;

        /// <summary>
        /// Maximum arc angle in radians 
        /// </summary>
        private readonly double minimumAngleDifference;


        /// <summary>
        /// A component that attempts to detect clustered stops in a trip and outputs segmented trips
        /// </summary>
        /// <param name="maximumDwellSpeed">Maximum speed in meters per second that qualifies as stopped</param>
        /// <param name="minimumDwellTime">Minimum time in seconds during which speed must be below maximumDwellSpeed for a stop to be detected</param>
        /// <param name="minimumAngleDifference">Minimum angle in radians with range [0, 2*PI) for each three point arc for it to be a stop</param>
        /// <param name="arcWindowSize">The arc size on which the average angle is calculated</param>
        public ClusteredStopDetectorModule(
            [Configurable(600)] int minimumDwellTime,
            [Configurable(1)] double maximumDwellSpeed,
            [Configurable(0.5)] double minimumAngleDifference,
            [Configurable(5)] int arcWindowSize)
        {
            if (maximumDwellSpeed < 0)
            {
                throw new ModuleConfigurationException("Maximum dwell speed must not be negative");
            }

            if (minimumDwellTime < 0)
            {
                throw new ModuleConfigurationException("Minimum dwell time must not be negative");
            }

            if (arcWindowSize < 1)
            {
                throw new ModuleConfigurationException("Arc window size must be a positive integer");
            }

            if (minimumAngleDifference < 0 || minimumAngleDifference >= Math.PI * 2)
            {
                throw new ModuleConfigurationException("Maximum angle difference must be in range  [0, 2*PI)");
            }

            this.maximumDwellSpeed = maximumDwellSpeed;
            this.minimumDwellTime = minimumDwellTime;
            this.minimumAngleDifference = minimumAngleDifference;
            this.arcWindowSize = arcWindowSize;
        }

        protected override Task<TripDetectionContext> ProcessInternal(TripDetectionContext input)
        {
            var tripSegments = new List<TripSegmentBase>();

            foreach (var segment in input.TripSegments)
            {
                if (!segment.IsMovingSegment)
                {
                    tripSegments.Add(segment);
                    continue;
                }

                var currentStopCluster = new StoppedSegment();
                var currentMovingCluster = new MovingSegment();

                State previousState = State.Stopped;
                State currentState = State.Unknown;

                var points = segment.Points;
                currentStopCluster.Points.Add(points.First());

                // Loop over points and extract segments
                for (var i = 1; i < points.Count - 1; i++)
                {
                    var previousTrackingPoint = points[i - 1];
                    var currentTrackingPoint = points[i];
                    var nextTrackingPoint = points[i + 1];

                    double cumulativeAngle = 0;

                    for (int j = i - this.arcWindowSize /2 ; j <= i + this.arcWindowSize / 2; j++)
                    {
                        if (j >= 0 && j < points.Count)
                        {
                            cumulativeAngle += MathUtils.AngleBetweenPoints(points[j], points[i]);
                        }
                    }

                    cumulativeAngle /= this.arcWindowSize;

                    double calculatedSpeed = MathUtils.AverageSpeed(previousTrackingPoint, currentTrackingPoint, nextTrackingPoint);

                    currentState = (calculatedSpeed <= this.maximumDwellSpeed && cumulativeAngle  > this.minimumAngleDifference)
                        ? State.Stopped : State.Moving;

                    if (currentState == State.Moving)
                    {
                        currentMovingCluster.Points.Add(currentTrackingPoint);

                        if (previousState == State.Stopped)
                        {
                            if (currentStopCluster.GetDurationInSeconds() < this.minimumDwellTime)
                            {
                                tripSegments.Add(new MovingSegment(currentStopCluster.Points));
                            }
                            else
                            {
                                currentStopCluster.Points.Add(currentTrackingPoint);
                                tripSegments.Add(currentStopCluster);
                            }

                            currentStopCluster = new StoppedSegment();
                        }
                    }

                    if (currentState == State.Stopped)
                    {
                        currentStopCluster.Points.Add(currentTrackingPoint);

                        if (previousState == State.Moving)
                        {
                            currentMovingCluster.Points.Add(currentTrackingPoint);

                            tripSegments.Add(currentMovingCluster);

                            currentMovingCluster = new MovingSegment();
                        }
                    }

                    previousState = currentState;
                }


                // Add the last segment to the list of segments including the last
                // unprocessed point
                if (currentStopCluster.Points.Any())
                {
                    currentStopCluster
                        .Points
                        .Add(points[points.Count - 1]);

                    tripSegments.Add(currentStopCluster);
                }
                else if (currentMovingCluster.Points.Any())
                {
                    currentMovingCluster
                        .Points
                        .Add(points[points.Count - 1]);

                    tripSegments.Add(currentMovingCluster);
                }
            }

            input.TripSegments = tripSegments;

            return Task.FromResult(input);
        }

        private enum State
        {
            Moving,
            Stopped,
            Unknown
        }
    }
}
