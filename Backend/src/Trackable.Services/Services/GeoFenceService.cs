// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class GeoFenceService : CrudServiceBase<string, GeoFence, IGeoFenceRepository>, IGeoFenceService
    {
        private readonly INotificationService notificationService;
        private readonly IGeoFenceUpdateRepository geoFenceUpdateRepository;
        private readonly IAssetRepository assetRepository;

        public GeoFenceService(
            IGeoFenceRepository repository,
            IGeoFenceUpdateRepository geoFenceUpdateRepository,
            IAssetRepository assetRepository,
            INotificationService notificationService)
            : base(repository)
        {
            this.notificationService = notificationService;
            this.geoFenceUpdateRepository = geoFenceUpdateRepository;
            this.assetRepository = assetRepository;
        }

        public async Task<IEnumerable<string>> HandlePoints(string assetId, params IPoint[] points)
        {
            var notifiedFenceIds = new List<string>();
            var tasks = new List<Task>();

            var asset = await this.assetRepository.GetAsync(assetId);
            var fences = await this.repository.GetByAssetIdWithIntersectionAsync(asset.Id, points);
            if (!fences.Any())
            {
                return Enumerable.Empty<string>();
            }

            var updates = await this.geoFenceUpdateRepository.GetByGeofenceIdsAsync(asset.Id, fences.Select(f => f.Key.Id).ToList());

            foreach (var fence in fences.Keys)
            {
                var hasUpdate = updates.TryGetValue(fence.Id, out GeoFenceUpdate latestUpdate);

                // Continue if the cooldown period has yet to expire
                if (hasUpdate
                    && latestUpdate.UpdatedAt + TimeSpan.FromMinutes(fence.Cooldown) > DateTime.UtcNow
                    && latestUpdate.NotificationStatus == NotificationStatus.Triggered)
                {
                    continue;
                }

                var fenceStatus =
                    (fence.FenceType == FenceType.Inbound ^ fences[fence])
                        ? NotificationStatus.NotTriggered
                        : NotificationStatus.Triggered;

                // Continue if there are no updates to the status of the asset
                if (hasUpdate && latestUpdate.NotificationStatus == fenceStatus)
                {
                    continue;
                }

                // Update the status of the asset
                if (!hasUpdate)
                {
                    latestUpdate = new GeoFenceUpdate
                    {
                        NotificationStatus = fenceStatus,
                        GeoFenceId = fence.Id,
                        AssetId = asset.Id
                    };

                    tasks.Add(this.geoFenceUpdateRepository.AddAsync(latestUpdate));
                }
                else
                {
                    tasks.Add(this.geoFenceUpdateRepository.UpdateStatusAsync(latestUpdate.Id, fenceStatus));
                }

                // Continue if the status has moved from triggered to untriggered 
                if (fenceStatus != NotificationStatus.Triggered)
                {
                    continue;
                }

                tasks.AddRange(fence.EmailsToNotify.Select(email => notificationService.NotifyViaEmail(
                    email,
                    $"{fence.Name} Geofence was triggered by asset {asset.Name}",
                    "",
                    $"<strong>{fence.FenceType.ToString()}</strong> Geofence <strong>{fence.Name}</strong> was triggered by asset  <strong>{asset.Name}</strong> at  <strong>{DateTime.UtcNow.ToString("G")} (UTC)</strong>")));

                tasks.AddRange(fence.WebhooksToNotify.Select(webhook => notificationService.NotifyViaWebhook(
                    webhook,
                    new GeofenceWebhookNotification()
                    {
                        GeoFenceName = fence.Name,
                        GeoFenceType = fence.FenceType.ToString(),
                        AssetId = asset.Id,
                        TriggeredAtUtc = DateTime.UtcNow.ToString("G")
                    })));

                notifiedFenceIds.Add(fence.Id);
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }

            return notifiedFenceIds;
        }

        public async Task<IEnumerable<GeoFence>> FindByNameAsync(string name)
        {
            return await this.repository.FindByNameAsync(name);
        }

        public async Task<IEnumerable<GeoFence>> FindContainingAllTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAllTagsAsync(tags);
        }

        public async Task<IEnumerable<GeoFence>> FindContainingAnyTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAnyTagsAsync(tags);
        }
    }
}
