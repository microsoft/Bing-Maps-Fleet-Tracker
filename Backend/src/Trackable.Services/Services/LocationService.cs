// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class LocationService : CrudServiceBase<string, Location, ILocationRepository>, ILocationService
    {
        public LocationService(ILocationRepository repository)
            : base(repository)
        {
        }

        public async Task<IEnumerable<Location>> FindByNameAsync(string name)
        {
            return await this.repository.FindByNameAsync(name);
        }

        public async Task<IEnumerable<Location>> FindContainingAllTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAllTagsAsync(tags);
        }

        public async Task<IEnumerable<Location>> FindContainingAnyTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAnyTagsAsync(tags);
        }

        public async Task<IDictionary<string, int>> GetCountByAssetAsync(string locationId)
        {
            return await this.repository.GetCountPerAssetAsync(locationId);
        }
    }
}
