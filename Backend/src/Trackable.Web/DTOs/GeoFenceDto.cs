using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Web.DTOs
{
    public class GeoFenceDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> EmailsToNotify { get; set; }

        public long Cooldown { get; set; }

        public FenceType FenceType { get; set; }

        public GeoFenceAreaType AreaType { get; set; }

        public IEnumerable<Point> FencePolygon { get; set; }

        public Point FenceCenter { get; set; }

        public double? RadiusInMeters { get; set; }

        public IEnumerable<string> AssetIds { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
