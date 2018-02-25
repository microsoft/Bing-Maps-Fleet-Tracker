using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Web.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
