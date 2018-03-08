// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;
using ZXing.QrCode;

namespace Trackable.Services
{
    class TrackingDeviceService : CrudServiceBase<string, TrackingDevice, ITrackingDeviceRepository>, ITrackingDeviceService
    {
        public TrackingDeviceService(ITrackingDeviceRepository repository)
            : base(repository)
        {
        }

        public async Task<TrackingDevice> AddOrUpdateDeviceAsync(TrackingDevice device)
        {
            var foundDevice = await this.GetAsync(device.Id);
            var foundDeviceByName = await this.FindByNameAsync(device.Name);

            if (foundDevice == null && !foundDeviceByName.Any())
            {
                return await AddAsync(device);
            }
            else
            {
                if (string.IsNullOrEmpty(device.AssetId))
                {
                    device.AssetId = foundDevice.AssetId;
                }

                return await UpdateAsync(device.Id, device);
            }
        }

        public byte[] GetDeviceProvisioningQrCode(PhoneClientData data, int height, int width, int margin)
        {
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions { Height = height, Width = width, Margin = margin }
            };

            var pixelData = qrCodeWriter.Write(JsonConvert.SerializeObject(data));
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            using (var memoryStream = new MemoryStream())
            {

                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                   ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image 
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                       pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                // save to stream as PNG
                bitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        public async Task<IDictionary<string, TrackingPoint>> GetDevicesLatestPositions()
        {
            return await this.repository.GetDevicesLatestPositions();
        }

        public async Task<IEnumerable<TrackingDevice>> FindByNameAsync(string name)
        {
            return await this.repository.FindByNameAsync(name);
        }

        public async Task<IEnumerable<TrackingDevice>> FindContainingAllTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAllTagsAsync(tags);
        }

        public async Task<IEnumerable<TrackingDevice>> FindContainingAnyTagsAsync(IEnumerable<string> tags)
        {
            return await this.repository.FindContainingAnyTagsAsync(tags);
        }
    }
}
