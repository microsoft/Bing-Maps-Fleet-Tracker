// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("Assets")]
    public class AssetData : EntityBase<string>, ITaggedEntity, INamedEntity
    {
        public string Name { get; set; }

        public TrackingDeviceData TrackingDevice { get; set; }

        public ICollection<GeoFenceUpdateData> NotificationHistory { get; set; }

        public ICollection<GeoFenceData> GeoFenceDatas { get; set; }

        [Required]
        public int AssetType { get; set; }

        public AssetPropertiesData AssetProperties { get; set; }

        public TrackingPointData LatestPosition { get; set; }

        public ICollection<TagData> Tags { get; set; }
    }
}
