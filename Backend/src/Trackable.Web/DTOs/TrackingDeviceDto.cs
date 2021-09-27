// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace Trackable.Web.Dtos
{
    public class TrackingDeviceDto
    {
        /// <summary>
        /// Device Id
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Device name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Model of the device
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Phone number of the device
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Operating System of the device, eg. Android, iOS
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Version of the Operating System of the device
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Id of the asset linked with this device.
        /// Can be changed to relink to another device.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The asset linked with this device.
        /// </summary>
        public NestedAssetDto Asset { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
