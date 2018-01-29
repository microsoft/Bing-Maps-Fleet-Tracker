using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.TripDetection
{
    [Serializable]
    public class AzurePipelineState
    {
        public int TripDetectorType { get; set; }

        public int ModuleIndex { get; set; }

        public object Payload { get; set; }
    }
}
