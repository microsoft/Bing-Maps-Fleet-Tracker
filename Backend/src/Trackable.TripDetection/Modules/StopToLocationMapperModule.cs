// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Configurations;
using Trackable.Models;
using Trackable.Repositories;
using Trackable.TripDetection.Exceptions;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that matches stop to already known locations if they are within a specified distance
    /// of each other. Prioritizes closest locations. Creates new locations for unmatched stops.
    /// </summary>
    class StopToLocationMapperModule : ModuleBase<TripDetectionContext, TripDetectionContext>
    {
        private readonly ILocationRepository locationRepository;

        private IList<Location> locations;

        /// <summary>
        /// The maximum allowed distance in meters between a calculated stop point and a location for matching
        /// </summary>
        private readonly double maximumDistanceCenterToLocation;

        /// <summary>
        /// A component that matches detected stops to known locations
        /// </summary>
        /// <param name="locationRepository">Repository of locations</param>
        /// <param name="maximumDistanceCenterToLocation">Maximum distance in meters between stops and locations for matching</param>
        public StopToLocationMapperModule(
            ILocationRepository locationRepository,
            [Configurable(500)] double maximumDistanceCenterToLocation)
        {
            if (maximumDistanceCenterToLocation < 0)
            {
                throw new ModuleConfigurationException("Distance to location cannot be negative");
            }

            this.locationRepository = locationRepository.ThrowIfNull(nameof(locationRepository));

            this.maximumDistanceCenterToLocation = maximumDistanceCenterToLocation;
        }

        public async override Task<TripDetectionContext> Process(TripDetectionContext input, ILogger logger)
        {
            var stopClusters = input
                .TripSegments
                .Where(t => !t.IsMovingSegment);

            logger.LogDebugSerialize("Recieved stop clusters {0}", stopClusters);

            if (!stopClusters.Any())
            {
                return input;
            }

            //Todo: Limit this query to a reasonable radius
            this.locations = (await this.locationRepository.GetAllAsync()).ToList();

            logger.LogDebugSerialize("Loaded locations {0}", this.locations);

            foreach (StoppedSegment stop in stopClusters)
            {
                var startLocation = await GetOrCreateLocation(stop.Points.First());
                stop.StartLocationId = startLocation.Id;

                var endLocation = await GetOrCreateLocation(stop.Points.Last());
                stop.EndLocationId = endLocation.Id;
            }

            logger.LogDebugSerialize("Assigned stops to locations {0}", stopClusters);

            return input;
        }


        private async Task<Location> GetOrCreateLocation(IPoint estimatedPosition)
        {
            var nearestLocation =
                    (from l in this.locations
                     let distance = MathUtils.DistanceInMeters(estimatedPosition, l)
                     where distance <= this.maximumDistanceCenterToLocation
                     orderby distance
                     select l).FirstOrDefault();

            if (nearestLocation == null)
            {
                nearestLocation = new Location();
                nearestLocation.Name = "Auto-Generated Location";
                nearestLocation.Longitude = estimatedPosition.Longitude;
                nearestLocation.Latitude = estimatedPosition.Latitude;
                nearestLocation.Address = "Auto-Generated";
                nearestLocation.InterestLevel = InterestLevel.AutoNew;

                var addedLocation = await this.locationRepository.AddAsync(nearestLocation);
                locations.Add(addedLocation);

                nearestLocation = addedLocation;
            }

            return nearestLocation;
        }
    }
}
