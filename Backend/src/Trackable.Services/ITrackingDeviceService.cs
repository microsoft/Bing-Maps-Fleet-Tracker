using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Repositories;
using Trackable.Models;

namespace Trackable.Services
{
    public interface ITrackingDeviceService : ICrudService<string, TrackingDevice>
    {
        Task<TrackingDevice> GetDeviceByNameAsync(string name);

        Task<TrackingDevice> AddOrUpdateDeviceAsync(TrackingDevice device);

        byte[] GetDeviceProvisioningQrCode(PhoneClientData data, int height, int width, int margin);

        Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions();
        
    }
}
