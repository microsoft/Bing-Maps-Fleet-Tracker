// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Trackable.Models;

namespace Trackable.Repositories
{
    /// <summary>
    /// Responsible for handling the configuration repository.
    /// </summary>
    public interface IConfigurationRepository : IRepository<string, string, Configuration>
    {
    }
}
