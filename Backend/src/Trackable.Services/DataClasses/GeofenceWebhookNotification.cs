// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;

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
