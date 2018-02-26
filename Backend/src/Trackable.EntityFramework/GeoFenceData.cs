using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("GeoFences")]
    public class GeoFenceData : EntityBase<int>, ITaggedEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Emails { get; set; }

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