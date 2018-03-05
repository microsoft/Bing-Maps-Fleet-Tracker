// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Repositories;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    /// <summary>
    /// Loads the tracking points from the database, sorts them, and passes them
    /// to the pipeline in the correct format
    /// </summary>
    class PointLoaderModule : ModuleBase<string, TripDetectionContext>
    {
        private readonly ITrackingPointRepository trackingPointRepository;

        public PointLoaderModule(ITrackingPointRepository trackingPointRepository)
        {
            this.trackingPointRepository =
                trackingPointRepository.ThrowIfNull(nameof(trackingPointRepository));
        }

        public async override Task<TripDetectionContext> Process(string assetId, ILogger logger)
        {
            logger.LogDebugSerialize("Loading points for asset with id {0}", assetId);

            var latestPoint =
                await this.trackingPointRepository.GetByAssetIdLastLabeledAsync(assetId);

            logger.LogDebugSerialize("Latest point found {0}", latestPoint);

            var startingTime = latestPoint == null ? DateTime.MinValue : latestPoint.CreatedAtUtc;

            var points =
                (await this.trackingPointRepository.GetByAssetIdAfterDateAsync(
                    assetId,
                    startingTime,
                    false)).ToList();

            logger.LogDebugSerialize("Loaded points {0}", points);

            var result = points
                .OrderBy(p => p.DeviceTimestampUtc)
                .ToList();

            logger.LogDebugSerialize("Ordered points {0}", result);

            return new TripDetectionContext
            {
                OriginalPoints = points,
                FilteredOrderedPoints = result,
                TripSegments = null,
                ResultantTrips = null
            };
        }
    }
}
