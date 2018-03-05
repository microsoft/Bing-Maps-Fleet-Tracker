// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.TripDetection
{
    public interface IModule
    {
        Task<object> Run(object input, ILogger logger);

        Type GetInputType();

        Type GetOutputType();
    }

    public interface IModule<TIn, TOut> : IModule
    {
        Task<TOut> Run(TIn input, ILogger logger);
    }
}
