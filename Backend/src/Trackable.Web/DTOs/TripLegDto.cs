using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Web.DTOs
{
    public class TripLegDto
    {
        public LocationDto EndLocation { get; set; }

        public LocationDto StartLocation { get; set; }

        public double AverageSpeed { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public IEnumerable<Point> Route { get; set; }
    }
}
