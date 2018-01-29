using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("TripLegs")]
    public class TripLegData : EntityBase<int>
    {
        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public DbGeography Route { get; set; }

        public double AverageSpeed { get; set; }

        public int TripDataId { get; set; }

        public TripData Trip { get; set; }

        public int StartLocationId { get; set; }

        public int EndLocationId { get; set; }

        public LocationData StartLocation { get; set; }

        public LocationData EndLocation { get; set; }
    }
}