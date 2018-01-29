using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.TripDetection.Modules;

namespace Trackable.TripDetection
{
    public interface IPipeline
    {
        Task<object> ExecuteModules(IEnumerable<IModuleLoader> moduleLoaders, object input);

        Task<object> ExecuteModule(IModuleLoader moduleLoader, object input);
    }
}
