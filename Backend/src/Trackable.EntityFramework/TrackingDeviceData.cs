// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("TrackingDevices")]
    public class TrackingDeviceData : EntityBase<string>, ITaggedEntity, INamedEntity
    {
        public string Name { get; set; }

        public string Model { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string OperatingSystem { get; set; }

        public string Version { get; set; }

        public AssetData Asset { get; set; }

        public TrackingPointData LatestPosition { get; set; }

        public ICollection<TagData> Tags { get; set; }

        public ICollection<TokenData> Tokens { get; set; }
    }
}