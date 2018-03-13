// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.TripDetection.Exceptions;
using Trackable.Configurations;
using Trackable.Repositories;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// A component that segments a stream of labeled points into trips, based on the
    /// time spent in each stop.
    /// </summary>
    class PoiTripExtractorModule : TripExtractorBase
    {
        private readonly ILocationRepository locationRepository;

        /// <summary>
        /// Dwell time after which we consider the stop the end of a trip even
        /// if its not close to a POI
        /// </summary>
        private readonly long timeoutDwellTime;

        /// <summary>
        /// Minimum interest level for location to be qualified as POI
        /// </summary>
        private readonly InterestLevel minimumInterestLevel;

        /// <summary>
        /// Private variable used to store the ids of the intersting locations
        /// </summary>
        private IEnumerable<string> interestingLocationIds;

        protected override Predicate<StoppedSegment> isEndOfTrip => stop =>
        {
            return stop.GetDurationInSeconds() > this.timeoutDwellTime
                    || interestingLocationIds.Contains(stop.StartLocationId);
        };

        /// <summary>
        /// Component that segments stream of points into trips knowing
        /// the fact that trips should only end at "interesting" locations
        /// </summary>
        public PoiTripExtractorModule(
            ILocationRepository locationRepository,
            [Configurable(InterestLevel.Manual)] InterestLevel minimumInterestLevel,
            [Configurable(1200)] long timeoutDwellTime,
            [Configurable(5)] int minimumTripPoints,
            [Configurable(3000)] double minimumTripDistance)
            : base(minimumTripPoints, minimumTripDistance)
        {
            if (timeoutDwellTime < 0)
            {
                throw new ModuleConfigurationException("Timeout dwell time must not be negative");
            }

            this.locationRepository = locationRepository.ThrowIfNull(nameof(locationRepository));
            this.timeoutDwellTime = timeoutDwellTime;
            this.minimumInterestLevel = minimumInterestLevel;
        }

        protected async override Task OnProcessCalled(TripDetectionContext input)
        {
            var locationIds = input.TripLegCandidates.Select(t => t.LastStoppedSegment.StartLocationId);
            var locations = await this.locationRepository.GetAsync(locationIds);
            interestingLocationIds = locations
                .Where(l => l.InterestLevel >= this.minimumInterestLevel)
                .Select(l => l.Id);
        }
    }
}
