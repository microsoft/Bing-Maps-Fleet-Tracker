using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("GeoFenceUpdates")]
    public class GeoFenceUpdateData : EntityBase<int>
    {
        [Required]
        public int GeoFenceDataId { get; set; }

        [Required]
        public string AssetDataId { get; set; }

        [Required]
        public int Status { get; set; }
    }
}