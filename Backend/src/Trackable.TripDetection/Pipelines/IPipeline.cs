// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.TripDetection.Modules;

namespace Trackable.TripDetection
{
    public interface IPipeline
    {
        Task<object> ExecuteModules(IEnumerable<IModuleLoader> moduleLoaders, object input);

        Task<object> ExecuteModule(IModuleLoader moduleLoader, object input);
    }
}
