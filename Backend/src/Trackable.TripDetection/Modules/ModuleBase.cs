// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.Repositories;
using Trackable.Models;
using Trackable.TripDetection.Helpers;

namespace Trackable.TripDetection.Components
{
    internal abstract class ModuleBase<TIn, TOut> : IModule<TIn, TOut>
    {
        public abstract Task<TOut> Process(TIn input, ILogger logger);

        public async Task<object> Run(object input, ILogger logger)
        {
            return await this.Run((TIn)input, logger);
        }

        public async Task<TOut> Run(TIn input, ILogger logger)
        {
            if (input == null)
            {
                throw new ArgumentNullException("Cannot provide component with null input");
            }

            return await Process(input, logger);
        }

        public Type GetInputType()
        {
            return typeof(TIn);
        }

        public Type GetOutputType()
        {
            return typeof(TOut);
        }
    }
}
