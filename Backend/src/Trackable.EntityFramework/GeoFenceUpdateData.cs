// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("GeoFenceUpdates")]
    public class GeoFenceUpdateData : EntityBase<int>
    {
        [Required]
        [Index("IX_GeoFenceDataId")]
        [Index("GeoFenceAssetUnique", IsUnique = true, Order = 0)]
        public string GeoFenceDataId { get; set; }

        [Required]
        [Index("IX_AssetDataId")]
        [Index("GeoFenceAssetUnique", IsUnique = true, Order = 1)]
        public string AssetDataId { get; set; }

        [Required]
        public int Status { get; set; }
    }
}