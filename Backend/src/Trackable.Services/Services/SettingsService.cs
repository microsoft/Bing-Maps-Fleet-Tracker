// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trackable.Common;
using Trackable.Models;
using Trackable.Common.Exceptions;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Trackable.Services
{
    class SettingsService : ISettingsService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public SettingsService(IConfiguration configuration)
        {
            this.configuration = configuration.ThrowIfNull(nameof(configuration));

            if (Boolean.Parse(this.configuration["Versioning:CheckForUpdates"]))
            {
                this.httpClient = new HttpClient
                {
                    BaseAddress = new Uri(this.configuration["Versioning:CheckUrl"])
                };
            }
        }

        public IEnumerable<SubscriptionKey> GetSubscriptionKeys()
        {
            return this.configuration
                .GetSection("SubscriptionKeys")
                .GetChildren()
                .Select(c => new SubscriptionKey { Id = c.Key, KeyValue = c.Value });
        }

        public async Task<UpdateStatus> GetUpdateStatus()
        {
            if (!Boolean.Parse(this.configuration["Versioning:CheckForUpdates"]))
            {
                return new UpdateStatus();
            }

            var latestVersionRequest = await this.httpClient.GetAsync(this.configuration["Versioning:CheckUrl"]);

            if (!latestVersionRequest.IsSuccessStatusCode)
            {
                throw new ResourceNotFoundException("Unable to retrieve latest version of project hurghada");
            }

            var versionJson = await latestVersionRequest.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<JObject>(versionJson);
           
            return new UpdateStatus
            {
                CurrentVersionHash = this.configuration["Versioning:VersionHash"],
                CurrentVersionName = this.configuration["Versioning:VersionName"],
                LatestVersionHash = jsonObject["versionHash"].ToString(),
                LatestVersionName = jsonObject["versionName"].ToString(),
                UpdateUrl = new Uri(this.configuration["Versioning:UpdateUrl"])
            };
        }
    }
}
