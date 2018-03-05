// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface IAssetPropertiesRepository : IRepository<int, AssetProperties>
    {
        Task CheckValidity(AssetProperties assetProperties);
    }
}
