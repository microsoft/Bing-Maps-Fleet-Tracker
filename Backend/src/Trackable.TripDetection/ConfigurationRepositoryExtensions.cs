using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.TripDetection
{
    internal static class ConfigurationExtensions
    {
        public static string GetAssemblyNamespace()
        {
            return "TripDetection";
        }

        public static Task<Configuration> GetTripDetectionConfigurationAsync(this IConfigurationRepository configurationRepository, string key)
        {
            return configurationRepository.GetAsync(GetAssemblyNamespace(), key);
        }
    }
}
