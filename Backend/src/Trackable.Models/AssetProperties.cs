// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Trackable.Models.Helpers;

namespace Trackable.Models
{
    public class AssetProperties : ModelBase<int>
    {
        [Mutable]
        public double? AssetHeight { get; set; }

        [Mutable]
        public double? AssetWidth { get; set; }

        [Mutable]
        public double? AssetLength { get; set; }

        [Mutable]
        public double? AssetWeight { get; set; }

        [Mutable]
        public int? AssetAxels { get; set; }

        [Mutable]
        public int? AssetTrailers { get; set; }

        [Mutable]
        public bool? AssetSemi { get; set; }

        [Mutable]
        public double? AssetMaxGradient { get; set; }

        [Mutable]
        public double? AssetMinTurnRadius { get; set; }
    }
}
