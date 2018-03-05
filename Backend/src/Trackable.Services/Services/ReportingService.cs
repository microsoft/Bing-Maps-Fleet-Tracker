// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class ReportingService : IReportingService
    {
        private readonly ITripRepository tripRepository;
        private readonly ITrackingPointRepository trackingPointRepository;
        private readonly IGeoFenceUpdateRepository geoFenceUpdateRepository;

        public ReportingService(
            ITripRepository tripRepository,
            ITrackingPointRepository trackingPointRepository,
            IGeoFenceUpdateRepository geoFenceUpdateRepository)
        {
            this.tripRepository = tripRepository.ThrowIfNull(nameof(tripRepository));
            this.trackingPointRepository = trackingPointRepository.ThrowIfNull(nameof(trackingPointRepository));
            this.geoFenceUpdateRepository = geoFenceUpdateRepository.ThrowIfNull(nameof(geoFenceUpdateRepository));
        }

        public async Task<IEnumerable<Metric>> GetAssetsMetricsAsync()
        {
            var metrics = new List<Metric>()
            {
                new Metric ("Total number of trips", "Trips", "Asset"),
                new Metric ("Total trip duration", "Minutes", "Asset"),
                new Metric ("Average trip duration", "Minutes", "Asset"),
                new Metric ("Total trip mileage", "Kilometers", "Asset"),
                new Metric ("Average trip mileage", "Kilometers", "Asset"),
                new Metric ("Points collected per asset", "Points", "Asset"),
                new Metric ("Geofences triggered per asset", "Geofences", "Asset"),
                new Metric ("Trips per day", "Trips", "Day"),
                new Metric ("Detected trips per day", "Trips", "Day"),
                new Metric ("Points collected per day", "Points", "Day"),
                new Metric ("Geofences triggered per day", "Geofences", "Day"),
            };

            var metricsDict = metrics.ToDictionary(m => m.Name, m => m);

            var trips = await this.tripRepository.GetAllAsync();

            var groupedTripsByAsset = trips.GroupBy(t => t.AssetId);
            foreach (var group in groupedTripsByAsset)
            {
                var groupCount = group.Count();
                var durationInMinutes = group.Sum(t => t.DurationInMinutes);
                var totalDistance = group.Sum(t => t.TripLegs.Sum(l => l.AverageSpeed * (l.EndTimeUtc - l.StartTimeUtc).TotalSeconds)) / 1000;

                metricsDict["Total number of trips"]
                    .Values
                    .Add(group.Key, groupCount);

                metricsDict["Total trip duration"]
                    .Values
                    .Add(group.Key, durationInMinutes);

                metricsDict["Average trip duration"]
                    .Values
                    .Add(group.Key, durationInMinutes / groupCount);

                metricsDict["Total trip mileage"]
                    .Values
                    .Add(group.Key, totalDistance);

                metricsDict["Average trip mileage"]
                    .Values
                    .Add(group.Key, totalDistance / groupCount);
            }

            var groupedTripsByStartDate = trips.GroupBy(t => t.StartTimeUtc.Date);
            foreach (var group in groupedTripsByStartDate)
            {
                metricsDict["Trips per day"]
                    .Values
                    .Add(group.Key.ToShortDateString(), group.Count());
            }

            var groupedTripsByCreatedDate = trips.GroupBy(t => t.CreatedAtUtc.Date);
            foreach (var group in groupedTripsByCreatedDate)
            {
                metricsDict["Detected trips per day"]
                    .Values
                    .Add(group.Key.ToShortDateString(), group.Count());
            }

            var pointsPerDayCounts = await this.trackingPointRepository.PointsPerDayCountAsync();
            foreach (var day in pointsPerDayCounts)
            {
                metricsDict["Points collected per day"]
                    .Values
                    .Add(day.Key.ToShortDateString(), day.Value);
            }

            var countPerAsset = await this.trackingPointRepository.PointsPerAssetCountAsync();
            foreach (var assetCount in countPerAsset)
            {
                metricsDict["Points collected per asset"]
                    .Values
                    .Add(assetCount.Key, assetCount.Value);
            }

            var triggerNotifications = await this.geoFenceUpdateRepository.GetAllAsync();

            var triggersPerAsset = triggerNotifications
                .Where(t => t.NotificationStatus == NotificationStatus.Triggered).GroupBy(n => n.AssetId);
            foreach (var asset in triggersPerAsset)
            {
                metricsDict["Geofences triggered per asset"]
                    .Values
                    .Add(asset.Key, asset.Count());
            }

            var triggersPerDay = triggerNotifications
                .Where(t => t.NotificationStatus == NotificationStatus.Triggered).GroupBy(n => n.UpdatedAt.Date);
            foreach (var day in triggersPerDay)
            {
                metricsDict["Geofences triggered per day"]
                    .Values
                    .Add(day.Key.ToShortDateString(), day.Count());
            }

            return metrics;
        }
    }
}
