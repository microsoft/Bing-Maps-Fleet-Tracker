using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class GeoFenceService : CrudServiceBase<int, GeoFence, IGeoFenceRepository>, IGeoFenceService
    {
        private readonly INotificationService notificationService;
        private readonly IGeoFenceUpdateRepository geoFenceUpdateRepository;

        public GeoFenceService(
            IGeoFenceRepository repository,
            IGeoFenceUpdateRepository geoFenceUpdateRepository,
            INotificationService notificationService)
            : base(repository)
        {
            this.notificationService = notificationService;
            this.geoFenceUpdateRepository = geoFenceUpdateRepository;
        }

        public async override Task<IEnumerable<GeoFence>> AddAsync(IEnumerable<GeoFence> models)
        {
            var results = await base.AddAsync(models);

            var updatedList = new List<GeoFence>();
            foreach (var zipped in results.Zip(models, (r, m) => new { result = r, model = m }))
            {
                updatedList.Add(await this.repository.UpdateAssetsAsync(zipped.result, zipped.model.AssetIds));
            }

            return updatedList;
        }

        public async override Task<GeoFence> AddAsync(GeoFence model)
        {
            var result = await base.AddAsync(model);

            var updated = await this.repository.UpdateAssetsAsync(result, model.AssetIds);

            return updated;
        }

        public async override Task<IEnumerable<GeoFence>> UpdateAsync(IDictionary<int, GeoFence> models)
        {
            var updatedModels = await base.UpdateAsync(models);

            var updatedList = new List<GeoFence>();
            foreach (var updatedModel in updatedModels)
            {
                updatedList.Add(await this.repository.UpdateAssetsAsync(updatedModel, models[updatedModel.Id].AssetIds));
            }

            return updatedList;
        }

        public async override Task<GeoFence> UpdateAsync(int geoFenceId, GeoFence model)
        {
            var updatedModel = await base.UpdateAsync(geoFenceId, model);

            return await this.repository.UpdateAssetsAsync(updatedModel, model.AssetIds);
        }

        public async Task<IEnumerable<int>> HandlePoints(string assetId, params IPoint[] points)
        {
            var notifiedFenceIds = new List<int>();
            var tasks = new List<Task>();

            var fences = await this.repository.GetByAssetIdWithIntersectionAsync(assetId, points);
            if (!fences.Any())
            {
                return Enumerable.Empty<int>();
            }

            var updates = await this.geoFenceUpdateRepository.GetByGeofenceIdsAsync(assetId, fences.Select(f => f.Key.Id).ToList());

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
                        AssetId = assetId
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
                    $"{fence.Name} Geofence was triggered by asset {assetId}",
                    "",
                    $"<strong>{fence.FenceType.ToString()}</strong> Geofence <strong>{fence.Name}</strong> was triggered by asset  <strong>{assetId}</strong> at  <strong>{DateTime.UtcNow.ToString("G")} (UTC)</strong>")));

                tasks.AddRange(fence.WebhooksToNotify.Select(webhook => notificationService.NotifyViaWebhook(
                    webhook,
                    new GeofenceWebhookNotification()
                    {
                        GeoFenceName = fence.Name,
                        GeoFenceType = fence.FenceType.ToString(),
                        AssetId = assetId,
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
