// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Trackable.Common;
using Trackable.TripDetection.Exceptions;
using Trackable.Configurations;
using Trackable.TripDetection.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// Component that smoothes moving segments of trips to avoid having sharp edges or visible gps jitter
    /// </summary>
    class SmoothingFilterModule : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        /// <summary>
        /// Size of the window used for smoothing.
        /// </summary>
        private readonly int smoothingWindowSize;

        /// <summary>
        /// Component used for smoothing of the trips.
        /// </summary>
        /// <param name="smoothingWindowSize">Size of the smoothing window size. Must be an odd integer bigger than 1</param>
        public SmoothingFilterModule([Configurable(3)] int smoothingWindowSize)
        {
            if (smoothingWindowSize % 2 != 1 || smoothingWindowSize < 3)
            {
                throw new ModuleConfigurationException("Smoothing window size must be an odd integer larger than 1");
            }

            this.smoothingWindowSize = smoothingWindowSize;
        }

        public override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            var filteredSegments = input
                .TripSegments
                .Where(s => s.IsMovingSegment);

            logger.LogDebugSerialize("Recieved moving segments {0}", filteredSegments);
            
            foreach (var segment in filteredSegments)
            {
                MathUtils.SmoothPoints(segment.Points, this.smoothingWindowSize);
            }

            logger.LogDebugSerialize("Output moving segments {0}", filteredSegments);

            return Task.FromResult(input);
        }
    }
}
