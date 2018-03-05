// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.TripDetection.Modules;

namespace Trackable.TripDetection
{
    public interface ITripDetector
    {
        IEnumerable<IModuleLoader> GetModuleLoaders();

        TripDetectorType TripDetectorType { get; }
    }
}
