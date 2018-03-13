// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    [Serializable]
    internal class StoppedSegment : TripSegmentBase
    {
        public string StartLocationId { get; set; }

        public string EndLocationId { get; set; }

        public override bool IsMovingSegment
        {
            get
            {
                return false;
            }
        }

        public StoppedSegment()
        {
        }

        public StoppedSegment(IList<TrackingPoint> points)
            :base(points)
        {
        }
    }
}
