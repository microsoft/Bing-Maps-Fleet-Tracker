// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.TripDetection
{
    public class TripDetectorFactory : ITripDetectorFactory
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly ITripRepository tripRepository;
        private readonly ITrackingPointRepository trackingPointRepository;
        private readonly ILocationRepository locationRepository;
        private readonly string bingMapsKey;

        public TripDetectorFactory(
            IConfigurationRepository configurationRepository,
            ITripRepository tripRepository,
            ITrackingPointRepository trackingPointRepository,
            ILocationRepository locationRepository,
            string bingMapsKey)
        {
            this.configurationRepository = configurationRepository;
            this.tripRepository = tripRepository;
            this.trackingPointRepository = trackingPointRepository;
            this.locationRepository = locationRepository;
            this.bingMapsKey = bingMapsKey;
        }

        public async Task<ITripDetector> Create()
        {
            var configuration = await this.configurationRepository.GetTripDetectionConfigurationAsync(nameof(TripDetectorFactory));

            if (configuration == null)
            {
                configuration = await this.configurationRepository.AddAsync(
                    new Configuration(
                        ConfigurationExtensions.GetAssemblyNamespace(),
                        nameof(TripDetectorFactory),
                        "Determines which Trip detector will be used for trip detection",
                        TripDetectorType.SimpleHeuristic));
            }

            var type = configuration.GetValue<TripDetectorType>();

            return await Create(type);
        }

        public Task<ITripDetector> Create(TripDetectorType type)
        {
            if (type == TripDetectorType.SimpleHeuristic)
            {
                return Task.FromResult((ITripDetector)
                    new TimeBasedTripDetector(
                        this.configurationRepository,
                        this.locationRepository,
                        this.tripRepository,
                        this.trackingPointRepository,
                        this.bingMapsKey));
            }
            else if (type == TripDetectorType.SimplePointOfInterst)
            {
                return Task.FromResult((ITripDetector)
                    new PointOfInterestTripDetector(
                        this.configurationRepository,
                        this.locationRepository,
                        this.tripRepository,
                        this.trackingPointRepository,
                        this.bingMapsKey));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
