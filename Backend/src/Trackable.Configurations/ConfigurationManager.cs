// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Configurations
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly string namespacePrefix;
        private readonly string specificNamespace;

        public ConfigurationManager(IConfigurationRepository configurationRepository, string namespacePrefix, string specificNamespace)
        {
            this.configurationRepository = configurationRepository;
            this.namespacePrefix = namespacePrefix;
            this.specificNamespace = specificNamespace;
        }

        public async Task<T> LoadWithDefaultsAsync<T>(params object[] allParameters)
        {
            var type = typeof(T);

            var constructor = type.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Any(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(ConfigurableAttribute))));

            if (constructor == null)
            {
                constructor = type.GetConstructor(allParameters.Select(p => p.GetType()).ToArray());
            }

            if (constructor == null)
            {
                return (T)Activator.CreateInstance(type);
            }

            var parameters = constructor.GetParameters();
            var consturctorParametersList = new List<object>();
            var nonConfigurableParametersCounter = 0;
            foreach (var paramater in parameters)
            {
                var parameterValue = allParameters[nonConfigurableParametersCounter++];

                var config = await FindOrCreateConfiguration(type.Name, paramater.Name, parameterValue, defaultIsSpecific: true);
                consturctorParametersList.Add(config.GetValue(paramater.ParameterType));
            }

            return (T)constructor.Invoke(consturctorParametersList.ToArray());
        }


        public async Task<T> LoadAsync<T>(params object[] nonConfigurableParameters)
        {
            var type = typeof(T);

            var constructor = type.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Any(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(ConfigurableAttribute))));

            if (constructor == null)
            {
                constructor = type.GetConstructor(nonConfigurableParameters.Select(p => p.GetType()).ToArray());
            }

            if (constructor == null)
            {
                return (T)Activator.CreateInstance(type);
            }

            var parameters = constructor.GetParameters();
            var consturctorParametersList = new List<object>();
            var nonConfigurableParametersCounter = 0;
            foreach (var paramater in parameters)
            {
                var configurableAttribute = paramater.CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(ConfigurableAttribute));

                if (configurableAttribute != null)
                {
                    var defaultValue = configurableAttribute.ConstructorArguments[0].Value;
                    var description = configurableAttribute.ConstructorArguments[1].Value.ToString();

                    var config = await FindOrCreateConfiguration(type.Name, paramater.Name, defaultValue);
                    consturctorParametersList.Add(config.GetValue(paramater.ParameterType));
                }
                else
                {
                    consturctorParametersList.Add(nonConfigurableParameters[nonConfigurableParametersCounter++]);
                }
            }

            return (T)constructor.Invoke(consturctorParametersList.ToArray());
        }

        private async Task<Configuration> FindOrCreateConfiguration(
            string componentName,
            string parameterName,
            object defaultValue,
            string description = "",
            bool defaultIsSpecific = false)
        {
            var specificNamespace = $"{this.namespacePrefix}.{this.specificNamespace}.{componentName}";
            var commonNamespace = $"{this.namespacePrefix}.Common.{componentName}";

            var config = await this.configurationRepository.GetAsync(specificNamespace, parameterName);

            if (config == null)
            {
                config = await this.configurationRepository.GetAsync(commonNamespace, parameterName);
            }

            if (config == null)
            {
                config = new Configuration(defaultIsSpecific ? specificNamespace : commonNamespace,
                    parameterName, description, defaultValue);

                await this.configurationRepository.AddAsync(config);
            }

            return config;
        }
    }
}
