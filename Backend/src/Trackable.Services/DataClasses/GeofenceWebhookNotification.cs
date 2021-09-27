// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Services
{
    public class GeofenceWebhookNotification
    {
        public string GeoFenceName { get; set; }

        public string GeoFenceType { get; set; }

        public string AssetId { get; set; }

        public string TriggeredAtUtc { get; set; }
    }
}
