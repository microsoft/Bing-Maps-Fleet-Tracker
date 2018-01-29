using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.TripDetection.Exceptions;
using Trackable.TripDetection.Modules;

namespace Trackable.TripDetection
{
    public class Pipeline : IPipeline
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<Pipeline> pipelineLogger;

        public Pipeline(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
            this.pipelineLogger = loggerFactory.CreateLogger<Pipeline>();
        }

        public async Task<object> ExecuteModules(IEnumerable<IModuleLoader> moduleLoaders, object input)
        {
            pipelineLogger.LogInformation("Started executing all modules with input {0}", input);

            object moduleInput = input;
            foreach (var loader in moduleLoaders)
            {
                moduleInput = await ExecuteModule(loader, moduleInput);
            }

            pipelineLogger.LogInformation("Finished executing all modules with output {0}", moduleInput);
            return moduleInput;
        }

        public async Task<object> ExecuteModule(IModuleLoader moduleLoader, object input)
        {
            pipelineLogger.LogInformation("Started running module {0} with input {1}", moduleLoader.ModuleType(), input);

            var module = await moduleLoader.LoadModule();

            if (input.GetType() != module.GetInputType())
            {
                throw new PipelineDataTypeException($"Module input with type {input.GetType()} is not the required"
                    + $" type {module.GetInputType()}, for module {moduleLoader.ModuleType()}");
            }

            var runResult = await module.Run(input, loggerFactory.CreateLogger(moduleLoader.ModuleType()));

            pipelineLogger.LogInformation("Finished running module {0} with output {1}", moduleLoader.ModuleType(), runResult);

            return runResult;
        }
    }
}
