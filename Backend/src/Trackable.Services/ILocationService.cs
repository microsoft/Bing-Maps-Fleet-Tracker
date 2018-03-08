// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ILocationService : ICrudService<string, Location>
    {
        Task<IDictionary<string, int>> GetCountByAssetAsync(string locationId);

        Task<IEnumerable<Location>> FindByNameAsync(string name);

        Task<IEnumerable<Location>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<Location>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
