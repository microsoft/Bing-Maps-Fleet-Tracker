// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Trackable.Models
{
    public class TrackingDevice : ModelBase<string>, INamedModel, ITaggedModel
    {
        [Mutable]
        public string Name { get; set; }

        [Mutable]
        public string Model { get; set; }

        [Mutable]
        public string Phone { get; set; }

        [Mutable]
        public string OperatingSystem { get; set; }

        [Mutable]
        public string Version { get; set; }

        [Mutable]
        public string AssetId { get; set; }

        public Asset Asset { get; set; }

        [Mutable]
        public IEnumerable<string> Tags { get; set; }
    }
}
