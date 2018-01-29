using Trackable.Models.Helpers;

namespace Trackable.Models
{
    public class TrackingDevice : ModelBase<string>
    {
        [Mutable]
        public string Name { get; set; }

        [Mutable]
        public string Model { get; set; }

        [Mutable]
        public string Phone { get; set; }

        [Mutable]
        public string OperatingSystem { get; set; }

        [Mutable]
        public string Version { get; set; }

        [Mutable]
        public string AssetId { get; set; }
    }
}
