// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the location repository.
    /// </summary>
    public interface ILocationRepository : 
        IRepository<string, Location>, 
        IDbCountableRepository<string, LocationData, Location>,
        IDbNamedRepository<string, LocationData, Location>,
        IDbTaggedRepository<string, LocationData, Location>
    {
        Task<IEnumerable<Location>> GetAsync(IEnumerable<string> ids);

        Task<IDictionary<string, int>> GetCountPerAssetAsync(string locationId);

        Task<int> GetAutoLocationCountAsync();
    }
}
