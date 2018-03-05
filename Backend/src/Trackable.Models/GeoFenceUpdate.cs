// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Trackable.Models.Helpers;

namespace Trackable.Models
{
    public class GeoFenceUpdate : ModelBase<int>
    {
        [Mutable]
        public NotificationStatus NotificationStatus { get; set; } = NotificationStatus.Unknown;

        public DateTime UpdatedAt { get; set; }

        public int GeoFenceId { get; set; }

        public string AssetId { get; set; }
    }

    public enum NotificationStatus
    {
        Unknown = 0,
        Triggered = 10,
        NotTriggered = 20
    }
}
