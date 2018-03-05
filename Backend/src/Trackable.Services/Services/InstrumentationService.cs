// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Repositories;

namespace Trackable.Services
{
    class InstrumentationService : IInstrumentationService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly IAssetRepository assetRepository;

        private readonly IGeoFenceRepository geoFenceRepository;

        private readonly ILocationRepository locationRepository;

        private readonly ITrackingDeviceRepository trackingDeviceRepository;

        private readonly ITrackingPointRepository trackingPointRepository;

        private readonly IConfigurationRepository configurationRepository;

        private readonly IInstrumentationRepository instrumentationRepository;

        private readonly ILogger logger;

        private readonly IConfiguration configuration;

        public InstrumentationService(
            IAssetRepository assetRepository,
            IGeoFenceRepository geoFenceRepository,
            ILocationRepository locationRepository,
            ITrackingDeviceRepository trackingDeviceRepository,
            ITrackingPointRepository trackingPointRepository,
            IInstrumentationRepository instrumentationRepository,
            IConfigurationRepository configurationRepository,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            this.assetRepository = assetRepository;
            this.geoFenceRepository = geoFenceRepository;
            this.locationRepository = locationRepository;
            this.trackingDeviceRepository = trackingDeviceRepository;
            this.trackingPointRepository = trackingPointRepository;
            this.instrumentationRepository = instrumentationRepository;
            this.configurationRepository = configurationRepository;
            this.logger = loggerFactory.CreateLogger<InstrumentationService>();
            this.configuration = configuration.ThrowIfNull(nameof(configuration));
        }

        public async Task PostExceptionAsync(string exceptionMessage)
        {
            try
            {
                var approval = await this.GetInstrumentationApproval();
                if (approval.HasValue && approval.Value)
                {
                    await httpClient.PostAsync(
                        configuration["Instrumentation:ErrorLogUrl"] +
                        $"?DeploymentId={ await this.instrumentationRepository.GetDeploymentIdAsync() }" +
                        $"&SwVersion={ this.configuration["Versioning:VersionName"] }" +
                        $"&Message={ exceptionMessage }",
                        null);
                }
            }
            catch (Exception exception)
            {
                this.logger.LogWarning("Failed to send exception {0}", exception);
            }
        }

        public async Task PostWarningAsync(string exceptionMessage)
        {
            try
            {
                var approval = await this.GetInstrumentationApproval();
                if (approval.HasValue && approval.Value)
                {
                    await httpClient.PostAsync(
                        configuration["Instrumentation:WarningLogUrl"] +
                        $"?DeploymentId={ await this.instrumentationRepository.GetDeploymentIdAsync() }" +
                        $"&SwVersion={ this.configuration["Versioning:VersionName"] }" +
                        $"&Message={ exceptionMessage }",
                        null);
                }
            }
            catch (Exception exception)
            {
                this.logger.LogWarning("Failed to send warning {0}", exception);
            }
        }

        public async Task PostInstrumentationAsync()
        {
            try
            {
                var approval = await this.GetInstrumentationApproval();
                if (approval.HasValue && approval.Value)
                {
                    await httpClient.PostAsync(
                        configuration["Instrumentation:InstrumentationLogUrl"] +
                        $"?deploymentid={ await instrumentationRepository.GetDeploymentIdAsync() }" +
                        $"&AssetsCount={ await assetRepository.GetCountAsync() }" +
                        $"&ActiveAssetsCount={ await assetRepository.GetNumberOfActiveAssets(DateTime.Now - TimeSpan.FromDays(7)) }" +
                        $"&LocationsCount={ await locationRepository.GetCountAsync() }" +
                        $"&AutoLocationsCount={ await locationRepository.GetAutoLocationCountAsync()}" +
                        $"&TrackingDevicesCount={ await trackingDeviceRepository.GetCountAsync() }" +
                        $"&GeoFencesCount={ await geoFenceRepository.GetCountAsync() }" +
                        $"&TrackingPointsCount={ await trackingPointRepository.GetCountAsync() }",
                        null);
                }
            }
            catch (Exception exception)
            {
                this.logger.LogWarning("Failed to send instrumentation log {0} ", exception);
            }
        }

        public async Task<bool?> GetInstrumentationApproval()
        {
            var config = await this.configurationRepository.GetAsync("Instrumentation", "UserApproved");

            return config?.GetValue<bool>();
        }

        public async Task SetInstrumentationApproval(bool accepted)
        {
            var acceptanceConfig = new Models.Configuration("Instrumentation", "UserApproved", "Approval", accepted);
            var foundConfig = await this.configurationRepository.GetAsync(acceptanceConfig.Namespace, acceptanceConfig.Key);

            if (foundConfig != null)
            {
                await this.configurationRepository.UpdateAsync(acceptanceConfig.Namespace, acceptanceConfig.Key, acceptanceConfig);
            }
            else
            {
                await this.configurationRepository.AddAsync(acceptanceConfig);
            }
        }
    }
}
