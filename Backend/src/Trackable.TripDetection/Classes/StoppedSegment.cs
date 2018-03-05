// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    [Serializable]
    internal class StoppedSegment : TripSegmentBase
    {
        public int StartLocationId { get; set; }

        public int EndLocationId { get; set; }

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
