using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
