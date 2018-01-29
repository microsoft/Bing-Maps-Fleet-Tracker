using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Configurations
{
    interface IConfigurationManager
    {
        /// <summary>
        /// Creates an instance of a class that has constructor parameters marked with the
        /// configurable attribute. Configurable parameters take the corresponding configuration value
        /// in the database. If no value is present, a new value is created using the default specified
        /// in the configurable attribute. Parameters not marked with a configurable attribute must be 
        /// passed in order to this function
        /// </summary>
        /// <typeparam name="T">Type of class to be instantiated</typeparam>
        /// <param name="nonConfigurableParameters">Constructor parameters that dont have a configurable attribute in order</param>
        /// <returns>The async task containing the newly created instance</returns>
        Task<T> LoadAsync<T>(params object[] nonConfigurableParameters);

        /// <summary>
        /// Creates an instance of a class that has constructor parameters marked with the
        /// configurable attribute. Configurable parameters take the corresponding configuration value
        /// in the database. If no value is present, a new value is created using the default specified
        /// in the passed parameters. Defaults create using this method override defaults created from
        /// configurable attribute.
        /// </summary>
        /// <typeparam name="T">Type of class to be instantiated</typeparam>
        /// <param name="nonConfigurableParameters">All constructor parameters</param>
        /// <returns>The async task containing the newly created instance</returns>
        Task<T> LoadWithDefaultsAsync<T>(params object[] allParameters);
    }
}
