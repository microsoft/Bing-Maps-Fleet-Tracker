// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Trackable.TripDetection.Modules
{
    public interface IModuleLoader
    {
        Task<IModule> LoadModule();

        Type ModuleType();
    }
}
