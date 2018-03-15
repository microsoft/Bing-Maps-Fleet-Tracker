// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("GeoFences")]
    public class GeoFenceData : EntityBase<string>, ITaggedEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }

        public string Emails { get; set; }

        public string Webhooks { get; set; }

        [Required]
        public int FenceType { get; set; }

        [Required]
        public int AreaType { get; set; }

        [Required]
        public long CooldownInMinutes { get; set; }

        public DbGeography Polygon { get; set; }

        public double? Radius { get; set; }

        public ICollection<AssetData> AssetDatas { get; set; }

        public ICollection<GeoFenceUpdateData> NotificationHistory { get; set; }

        public ICollection<TagData> Tags { get; set; }
    }
}