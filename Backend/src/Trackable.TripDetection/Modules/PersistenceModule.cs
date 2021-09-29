// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Trackable.Common;
using Trackable.Repositories;
using Trackable.Models;
using Trackable.TripDetection.Helpers;
using BingMapsSDSToolkit.GeocodeDataflowAPI;
using BingMapsSDSToolkit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// Writes the output of the trip detection pipeline into the persistence store
    /// </summary>
    class PersistenceModule : ModuleBase<TripDetectionContext, IEnumerable<Trip>>
    {
        private readonly ITrackingPointRepository trackingPointRepository;

        private readonly ILocationRepository locationRepository;

        private readonly ITripRepository tripRepository;

        private readonly string bingMapsKey;

        public PersistenceModule(
            ITrackingPointRepository trackingPointRepository,
            ITripRepository tripRepository,
            ILocationRepository locationRepository,
            string bingMapsKey)
        {
            BingMapsSDSToolkit.InternalSettings.ClientApi = "bingmapsfleettracker";

            this.trackingPointRepository =
                trackingPointRepository.ThrowIfNull(nameof(trackingPointRepository));

            this.tripRepository =
                tripRepository.ThrowIfNull(nameof(tripRepository));

            this.locationRepository =
                locationRepository.ThrowIfNull(nameof(locationRepository));

            this.bingMapsKey = bingMapsKey;
        }

        public async override Task<IEnumerable<Trip>> Process(TripDetectionContext input, ILogger logger)
        {
            var savedTrips = new List<Trip>();

            var locationIdsToGeocode = new HashSet<string>();

            logger.LogDebugSerialize("Recieved trips {0}", input.ResultantTrips);

            foreach (var trip in input.ResultantTrips)
            {
                var savedTrip = await this.tripRepository.AddAsync(trip);

                var points = input.OriginalPoints.Where(p => p.TripId == trip.Id);

                await trackingPointRepository.AssignPointsToTripAsync(savedTrip.Id, points);

                logger.LogDebugSerialize("Assigned points {1} to trip {0}", points, trip);

                savedTrips.Add(savedTrip);

                foreach (var leg in trip.TripLegs)
                {
                    locationIdsToGeocode.Add(leg.StartLocationId);
                    locationIdsToGeocode.Add(leg.EndLocationId);
                }
            }

            var locations = (await this.locationRepository.GetAllAsync())
                .Where(l => locationIdsToGeocode.Contains(l.Id) && l.Address == "Auto-Generated")
                .ToList();

            //Batch reverse geocode
            var geocodeFeed = new GeocodeFeed()
            {
                Entities = locations.Select((l, i) => new GeocodeEntity()
                {
                    ReverseGeocodeRequest = new ReverseGeocodeRequest()
                    {
                        Location = new GeodataLocation(l.Latitude, l.Longitude)
                    },
                    Id = i.ToString()
                }).ToList()
            };

            if (locations.Count > 0)
            {
                var geocodeManager = new BatchGeocodeManager();

                var res = await geocodeManager.Geocode(geocodeFeed, bingMapsKey);

                var locationsDict = new Dictionary<string, Location>();

                if (res.Succeeded != null)
                {
                    logger.LogDebugSerialize("Reverse Geocode result (Succeeded)", res.Succeeded);

                    foreach (var entity in res.Succeeded.Entities)
                    {
                        var loc = locations[int.Parse(entity.Id)];
                        loc.Address = entity.GeocodeResponse.First<GeocodeResponse>().Address.FormattedAddress;
                        loc.Name = entity.GeocodeResponse.First<GeocodeResponse>().Name;
                        await locationRepository.UpdateAsync(loc.Id, loc);
                    }
                }
                else
                {
                    logger.LogDebugSerialize("Reverse Geocode result (Failed)", res.Failed);
                    logger.LogDebugSerialize("Reverse Geocode result (Errors)", res.Error);
                }
            }
            logger.LogDebugSerialize("Saved trips output", savedTrips);

            return savedTrips;
        }
    }
}
