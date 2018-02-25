using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Web.DTOs
{
    public class AssetDto
    {
        public string Id { get; set; }

        public string TrackingDeviceId { get; set; }

        public string TrackingDeviceName { get; set; }

        public AssetType AssetType { get; set; }

        public AssetPropertiesDto AssetProperties { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
