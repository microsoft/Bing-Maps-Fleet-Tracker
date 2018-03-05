// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Trackable.Web.Dtos
{
    public class TripDto
    {
        public int Id { get; set; }

        public LocationDto StartLocation { get; set; }

        public LocationDto EndLocation { get; set; }

        public string AssetId { get; set; }

        public string TrackingDeviceId { get; set; }

        public IEnumerable<TripLegDto> TripLegs { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public double DurationInMinutes { get; set; }
    }
}
