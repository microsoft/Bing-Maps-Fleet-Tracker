
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("Dispatches")]
    public class DispatchData : EntityBase<int>
    {
       
        [Required]
        public string DeviceId { get; set; }
        
        public ICollection<DispatchPointData> Points { get; set; }

        public string AssetId { get; set; }

        public int? Optimize { get; set; }

        public double? LoadedHeight { get; set; }

        public double? LoadedWidth { get; set; }

        public double? LoadedLength { get; set; }

        public double? LoadedWeight { get; set; }

        public bool? AvoidCrossWind { get; set; }

        public bool? AvoidGroundingRisk { get; set; }
    }
}
