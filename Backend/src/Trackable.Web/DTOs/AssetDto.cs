// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackable.Models;

namespace Trackable.Web.Dtos
{
    public class AssetDto
    {
        /// <summary>
        /// The asset id
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// The id of the tracking device associated with this asset
        /// </summary>
        public string TrackingDeviceId { get; set; }

        /// <summary>
        /// The name of the tracking device associated with this asset
        /// </summary>
        public string TrackingDeviceName { get; set; }

        /// <summary>
        /// The asset type
        /// </summary>
        [Required]
        public AssetType AssetType { get; set; }

        /// <summary>
        /// Extended asset properties required if the asset is a truck
        /// </summary>
        public AssetPropertiesDto AssetProperties { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
