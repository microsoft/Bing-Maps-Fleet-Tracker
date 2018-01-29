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
    /// A component that marks points as noise based on unnaturally high speed, low reported accuracy,
    /// lack of a timestamp, or if they are outlies within a window.
    /// </summary>
    class NoiseRemovalModule : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        /// <summary>
        /// Minimum accuracy in meters needed for a point to count as non-Noise
        /// </summary>
        private readonly int minimumAccuracy;

        /// <summary>
        /// The acceleration in meters per second ^ 2 required to mark a point as noise.
        /// Needed for counteracting GPS jitter.
        /// </summary>
        private readonly double accelerationThreshold;

        /// <summary>
        /// Component used to mark points with low accuracy or unnaturally high speed as noise
        /// </summary>
        /// <param name="minimumGPSAccuracy">Minimum acceptable accuracy in meters reported by the GPS of the client.</param>
        /// <param name="accelerationThreshold">Acceleration in meters per second^2 above which points are marked as noise</param>
        public NoiseRemovalModule(
            [Configurable(70)] int minimumGPSAccuracy,
            [Configurable(10)] double accelerationThreshold)
        {
            if (accelerationThreshold <= 0)
            {
                throw new ModuleConfigurationException("Acceleration threshold must be a positive real number");
            }

            if (minimumGPSAccuracy <= 0)
            {
                throw new ModuleConfigurationException("Minimum GPS Accuracy threshold must be a positive integer");
            }

            this.minimumAccuracy = minimumGPSAccuracy;
            this.accelerationThreshold = accelerationThreshold;
        }


        public async override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            logger.LogDebugSerialize("Recieved points {0}", input.FilteredOrderedPoints);

            // Filter out points with low accuracy or no datetime stamps 
            var noisePoints = input.FilteredOrderedPoints.Where(p =>
                (p.Accuracy.HasValue && p.Accuracy.Value > this.minimumAccuracy)
                || p.DeviceTimestampUtc == 0);

            var preFilteredPoints = input.FilteredOrderedPoints.Except(noisePoints).ToList();

            logger.LogDebugSerialize("Filtered points based on accuracy or 0 timestamps {0}", preFilteredPoints);

            var filteredPoints = new List<TrackingPoint>();
            for (int i = 0; i < preFilteredPoints.Count - 1; i++)
            {
                if (preFilteredPoints[i].DeviceTimestampUtc != preFilteredPoints[i + 1].DeviceTimestampUtc
                    && (preFilteredPoints[i].Latitude != preFilteredPoints[i + 1].Latitude 
                    || preFilteredPoints[i].Longitude != preFilteredPoints[i + 1].Longitude ))
                {
                    filteredPoints.Add(preFilteredPoints[i]);
                }
            }

            input.FilteredOrderedPoints = filteredPoints;

            logger.LogDebugSerialize("Removed duplicate points {0}", filteredPoints);

            if (filteredPoints.Count < 2)
            {
                logger.LogDebugSerialize("Output few points {0}", filteredPoints);
                return input;
            }

            var accelerations = MathUtils.AveragePointAccelerations(filteredPoints);

            var flaggedPointIndeces = new HashSet<int>();
            for (int i = 0; i < accelerations.Count; i++)
            {
                if (accelerations[i] > this.accelerationThreshold)
                {
                    flaggedPointIndeces.Add(i);
                }
            }

            var finalPoints = new List<TrackingPoint>();
            for (int i = 0; i < filteredPoints.Count; i++)
            {
                if (!flaggedPointIndeces.Contains(i))
                {
                    finalPoints.Add(filteredPoints[i]);
                }
            }

            input.FilteredOrderedPoints = finalPoints.OrderBy(p => p.DeviceTimestampUtc).ToList();

            logger.LogDebugSerialize("Output further filtered points {0}", input.FilteredOrderedPoints);

            return input;
        }
    }
}
