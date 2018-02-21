using System.Collections.Generic;

namespace Trackable.Models
{
    public class Asset : ModelBase<string>, ITaggedModel
    {
        public string TrackingDeviceId { get; set; }
        
        public string TrackingDeviceName { get; set; }

        public AssetType AssetType { get; set; }

        public AssetProperties AssetProperties { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }

    public enum AssetType
    {
        Car,
        Truck
    }
}
