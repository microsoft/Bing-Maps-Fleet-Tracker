// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Models
{
    public class Asset : ModelBase<string>, ITaggedModel, INamedModel
    {
        [Mutable]
        public string Name { get; set; }

        public AssetType AssetType { get; set; }

        [Mutable]
        public AssetProperties AssetProperties { get; set; }

        [Mutable]
        public string TrackingDeviceId { get; set; }

        public TrackingDevice TrackingDevice { get; set; }

        [Mutable]
        public IEnumerable<string> Tags { get; set; }
    }

    public enum AssetType
    {
        Car,
        Truck
    }
}
