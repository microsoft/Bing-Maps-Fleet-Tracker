using System;

namespace Trackable.Models
{
    public class GeoFenceUpdate : ModelBase<int>
    {
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
