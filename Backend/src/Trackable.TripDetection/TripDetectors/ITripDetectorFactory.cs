using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.TripDetection
{
    public interface ITripDetectorFactory
    {
        Task<ITripDetector> Create();

        Task<ITripDetector> Create(TripDetectorType type);
    }
}
