// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITrackingDeviceService : ICrudService<string, TrackingDevice>
    {
        Task<TrackingDevice> GetDeviceByNameAsync(string name);

        Task<TrackingDevice> AddOrUpdateDeviceAsync(TrackingDevice device);

        byte[] GetDeviceProvisioningQrCode(PhoneClientData data, int height, int width, int margin);

        Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions();

        Task<IEnumerable<TrackingDevice>> FindByNameAsync(string name);

        Task<IEnumerable<TrackingDevice>> FindContainingAllTagsAsync(IEnumerable<string> tags);

        Task<IEnumerable<TrackingDevice>> FindContainingAnyTagsAsync(IEnumerable<string> tags);
    }
}
