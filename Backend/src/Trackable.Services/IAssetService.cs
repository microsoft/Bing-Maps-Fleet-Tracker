// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface IAssetService : ICrudService<string, Asset>
    {
        Task<IDictionary<string, TrackingPoint>> GetAssetsLatestPositions();

        Task<IEnumerable<Asset>> FindByNameAsync(string name);

        Task<IEnumerable<Asset>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<Asset>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
