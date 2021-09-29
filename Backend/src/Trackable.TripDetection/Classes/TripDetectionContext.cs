// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    [Serializable]
    internal class TripDetectionContext
    {
        public IEnumerable<TrackingPoint> OriginalPoints { get; set; }

        public IList<TrackingPoint> FilteredOrderedPoints { get; set; }

        public IList<TripSegmentBase> TripSegments { get; set; }

        public IList<TripLegCandidate> TripLegCandidates { get; set; }

        public IList<Trip> ResultantTrips { get; set; }
    }
}
