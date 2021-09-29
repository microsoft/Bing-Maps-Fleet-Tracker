// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Trackable.TripDetection
{
    public interface ITripDetectorFactory
    {
        Task<ITripDetector> Create();

        Task<ITripDetector> Create(TripDetectorType type);
    }
}
