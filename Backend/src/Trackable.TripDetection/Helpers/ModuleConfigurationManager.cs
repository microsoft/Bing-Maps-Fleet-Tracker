// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Trackable.Configurations;
using Trackable.Repositories;
using Trackable.TripDetection.Modules;

namespace Trackable.TripDetection.Helpers
{
    internal class ModuleConfigurationManager : ConfigurationManager
    {
        public ModuleConfigurationManager(
            IConfigurationRepository configurationRepository,
            string namespacePrefix,
            string specificNamespace) 
            : base(configurationRepository, namespacePrefix, specificNamespace)
        {
        }

        public ModuleLoader<T> LoadModuleAsync<T>(params object [] parameters) where T : IModule
        {
            return new ModuleLoader<T>(async () => await this.LoadAsync<T>(parameters));
        }

        public ModuleLoader<T> LoadModuleWithDefaultsAsync<T>(params object[] parameters) where T : IModule
        {
            return new ModuleLoader<T>(async () => await this.LoadWithDefaultsAsync<T>(parameters));
        }
    }
}
