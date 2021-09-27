// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

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
