// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.TripDetection.Helpers
{
    [Serializable]
    internal class TripLegCandidate
    {
        public StoppedSegment FirstStoppedSegment { get; set; } = new StoppedSegment();

        public MovingSegment MovingSegment { get; set; } = new MovingSegment();

        public StoppedSegment LastStoppedSegment { get; set; } = new StoppedSegment();
    }
}
