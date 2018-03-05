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
using Trackable.Configurations;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that segments a list of segments into a list of trip leg
    /// candidates.
    /// </summary>
    class LegCandidateExtractorModule : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        /// <summary>
        /// Minimum number of trip leg points
        /// </summary>
        private readonly int minimumNumberOfLegPoints;
        
        /// <summary>
        /// Component that extracts trip legs
        /// </summary>
        /// <param name="minimumNumberOfLegPoints">Minimum number of points a trip leg must have</param>
        public LegCandidateExtractorModule(
            [Configurable(3)] int minimumNumberOfLegPoints)
        {
            if(minimumNumberOfLegPoints < 0)
            {
                throw new ModuleConfigurationException("Minimum number of leg points cannot be negative");
            }

            this.minimumNumberOfLegPoints = minimumNumberOfLegPoints;
        }

        public async override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            var tripSegments = input.TripSegments;
            input.TripLegCandidates = new List<TripLegCandidate>();

            logger.LogDebugSerialize("Input trip segments {0}", tripSegments);

            var currentLeg = new TripLegCandidate();
            var previousLeg = new TripLegCandidate();

            for (int i = 0; i < tripSegments.Count - 2; i += 2)
            {
                if (tripSegments[i].IsMovingSegment)
                {
                    continue;
                }

                currentLeg.FirstStoppedSegment = (StoppedSegment)tripSegments[i];
                currentLeg.MovingSegment = (MovingSegment)tripSegments[i + 1];
                currentLeg.LastStoppedSegment = (StoppedSegment)tripSegments[i + 2];

                if (currentLeg.MovingSegment.Points.Count < this.minimumNumberOfLegPoints)
                {
                    if (previousLeg.LastStoppedSegment.Points.Any())
                    {
                        previousLeg.LastStoppedSegment.Points.AddRange(currentLeg.LastStoppedSegment.Points);
                        previousLeg.LastStoppedSegment.EndLocationId = currentLeg.LastStoppedSegment.EndLocationId;
                    }
                }
                else
                {
                    if (previousLeg.FirstStoppedSegment.Points.Any())
                    {
                        input.TripLegCandidates.Add(previousLeg);
                    }

                    previousLeg = currentLeg;
                    currentLeg = new TripLegCandidate();
                }
            }

            if (previousLeg.LastStoppedSegment.Points.Any())
            {
                input.TripLegCandidates.Add(previousLeg);
            }

            logger.LogDebugSerialize("Output trip candidates {0}", input.TripLegCandidates);

            return input;
        }

        private enum State
        {
            Moving,
            Stopped,
            Unknown
        }
    }
}
