// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITokenService : ICrudService<Guid, JwtToken>
    {
        Task<string> GetLongLivedUserToken(User user, bool regenerateToken);

        Task<string> GetLongLivedDeviceToken(TrackingDevice device, bool regenerateToken);

        string GetShortLivedDeviceRegistrationToken();

        Task DisableDeviceTokens(TrackingDevice device);

        Task DisableUserTokens(User user);
    }
}
