// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.TripDetection.Exceptions;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that segments a stream of candidate trip legs into trips, based on the
    /// time spent in each stop.
    /// </summary>
    abstract class TripExtractorBase : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        /// <summary>
        /// Minimum number of points for a trip 
        /// </summary>
        private readonly int miniumumTripPoints;

        /// <summary>
        /// Minimum distance covered by a trip
        /// </summary>
        private readonly double minimumTripDistance;

        /// <summary>
        /// Predicate that takes a stoppedSegment and decides if this marks the end
        /// of a trip
        /// </summary>
        protected abstract Predicate<StoppedSegment> isEndOfTrip { get; }

        /// <summary>
        /// Callback triggered when processing starts. Override to load extra data that
        /// will be used for the isEndOfTrip logic.
        /// </summary>
        protected async virtual Task onProcessCalled(TripDetectionContext input)
        {
        }

        /// <summary>
        /// Component that segments stream of points into trips
        /// </summary>
        /// <param name="miniumumTripPoints">Minimum number of points a trip can have</param>
        /// <param name="minimumTripDistance">Minimum distance a trip must cover </param>
        public TripExtractorBase(int miniumumTripPoints, double minimumTripDistance)
        { 
            if (miniumumTripPoints < 0)
            {
                throw new ModuleConfigurationException("Minimum trip points time must not be negative");
            }

            if (minimumTripDistance < 0)
            {
                throw new ModuleConfigurationException("Minimum trip dwell time must not be negative");
            }

            this.miniumumTripPoints = miniumumTripPoints;
            this.minimumTripDistance = minimumTripDistance;
        }

        public async override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            await onProcessCalled(input);

            logger.LogDebugSerialize("Recieved trip leg candidates {0}", input.TripLegCandidates);

            input.ResultantTrips = new List<Trip>();

            int tripId = 1;
            double distanceCoveredByTrip = 0;
            int numberOfMovingPoints = 0;
            var processedPoints = new List<TrackingPoint>();
            var currentLegs = new List<TripLeg>();
            foreach (var tripLeg in input.TripLegCandidates)
            {
                distanceCoveredByTrip += tripLeg.MovingSegment.GetBoundingRadius() * 2;
                numberOfMovingPoints += tripLeg.MovingSegment.Points.Count;

                processedPoints.AddRange(tripLeg.FirstStoppedSegment.Points);
                processedPoints.AddRange(tripLeg.LastStoppedSegment.Points);
                processedPoints.AddRange(tripLeg.MovingSegment.Points);

                var leg = GenerateTripLeg(tripLeg);
                currentLegs.Add(leg);

                // If the stop marking the end of the trip leg was matched the specified criteria,
                // Add current trip to the list of trips and reset variables for the next trip.
                if (isEndOfTrip(tripLeg.LastStoppedSegment))
                {
                    // If trip does not meet trip requirements, dont add to the resulting list of trips
                    if (numberOfMovingPoints >= this.miniumumTripPoints
                       && distanceCoveredByTrip >= this.minimumTripDistance)
                    {
                        var currentTrip = new Trip()
                        {
                            Id = tripId++,
                            AssetId = tripLeg.MovingSegment.Points.First().AssetId,
                            TrackingDeviceId = tripLeg.MovingSegment.Points.First().TrackingDeviceId,
                            EndLocationId = currentLegs.Last().EndLocationId,
                            EndTimeUtc = currentLegs.Last().EndTimeUtc,
                            StartLocationId = currentLegs.First().StartLocationId,
                            StartTimeUtc = currentLegs.First().StartTimeUtc,
                            TripLegs = currentLegs
                        };

                        processedPoints.ForEach(p => p.TripId = currentTrip.Id);
                        input.ResultantTrips.Add(currentTrip);
                    }

                    currentLegs = new List<TripLeg>();
                    processedPoints.Clear();
                    distanceCoveredByTrip = 0;
                    numberOfMovingPoints = 0;
                }
            }

            logger.LogDebugSerialize("Resulting trips {0}", input.ResultantTrips);

            return input;
        }

        private TripLeg GenerateTripLeg(TripLegCandidate tripLegCandidate)
        {
            return new TripLeg
            {
                AverageSpeed = tripLegCandidate.MovingSegment.GetAverageSpeed(),
                Route = tripLegCandidate.MovingSegment.Points,
                StartLocationId = tripLegCandidate.FirstStoppedSegment.EndLocationId,
                EndLocationId = tripLegCandidate.LastStoppedSegment.StartLocationId,
                StartTimeUtc = DateTimeUtils.FromUnixTime(
                    tripLegCandidate.MovingSegment.Points.First().DeviceTimestampUtc),
                EndTimeUtc = DateTimeUtils.FromUnixTime(
                    tripLegCandidate.MovingSegment.Points.Last().DeviceTimestampUtc)
            };
        }


        private enum State
        {
            Moving,
            Stopped,
            Unknown
        }
    }
}
