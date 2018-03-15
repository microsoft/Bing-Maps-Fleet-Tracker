// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("Trips")]
    public class TripData : EntityBase<int>
    {
        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        [Required]
        public string AssetId { get; set; }

        public AssetData Asset { get; set; }

        [Required]
        public string TrackingDeviceId { get; set; }

        public TrackingDeviceData TrackingDevice { get; set; }

        public ICollection<TrackingPointData> Points { get; set; }

        public ICollection<TripLegData> TripLegDatas { get; set; }

        public string StartLocationId { get; set; }

        public string EndLocationId { get; set; }

        public LocationData StartLocation { get; set; }

        public LocationData EndLocation { get; set; }
    }
}