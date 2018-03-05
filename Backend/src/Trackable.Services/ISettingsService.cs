// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ISettingsService
    {
        Task<UpdateStatus> GetUpdateStatus();

        IEnumerable<SubscriptionKey> GetSubscriptionKeys();
    }
}
