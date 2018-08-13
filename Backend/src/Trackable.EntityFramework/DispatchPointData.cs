
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("DispatchPoints")]
    public class DispatchPointData : EntityBase<int>
    {
        public DbGeography Location { get; set; }

        public int DispatchingDataId { get; set; }

        public DispatchData DispatchData { get; set; }
    }
}
