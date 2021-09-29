// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.TripDetection.Exceptions
{
    public class PipelineDataTypeException : Exception
    {
        public PipelineDataTypeException(string detailedMessage)
            :base(detailedMessage)
        {
        }
    }
}
