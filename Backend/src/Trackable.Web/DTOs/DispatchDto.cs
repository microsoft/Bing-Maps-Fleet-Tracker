using System;
using System.Collections.Generic;
using Trackable.Models;
using Trackable.Web.Dtos;

namespace Trackable.Web.Dtos
{
    public class DispatchDto
    {
        public string DeviceId { get; set; }

        public string AssetId { get; set; }

        public bool GetAlternativeCarRoute { get; set; }

        public IEnumerable<PointDto> WayPoints { get; set; }

        public int? MaxSolutions { get; set; }

        public IEnumerable<AvoidTypes> Avoid { get; set; }

        public int? DistanceBeforeFirstTurn { get; set; }

        public int? Heading { get; set; }

        public OptimizeValue? Optimize { get; set; }

        public IEnumerable<double> Tolerances { get; set; }

        public DistanceUnit? DistanceUnit { get; set; }

        public WeightUnit? WeightUnit { get; set; }

        public DimensionUnit? DimensionUnit { get; set; }

        public DateTime? DateTime { get; set; }

        public TimeType? TimeType { get; set; }

        public double? LoadedHeight { get; set; }

        public double? LoadedWidth { get; set; }

        public double? LoadedLength { get; set; }

        public double? LoadedWeight { get; set; }

        public bool? AvoidCrossWind { get; set; }

        public bool? AvoidGroundingRisk { get; set; }

        public IEnumerable<HazardousMaterial> HazardousMaterials { get; set; }

        public IEnumerable<HazardousMaterial> HazardousPermits { get; set; }
    }
}
