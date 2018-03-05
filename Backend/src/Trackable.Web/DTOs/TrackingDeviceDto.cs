// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Trackable.Web.Dtos
{
    public class TrackingDeviceDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Phone { get; set; }

        public string OperatingSystem { get; set; }

        public string Version { get; set; }

        public string AssetId { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
