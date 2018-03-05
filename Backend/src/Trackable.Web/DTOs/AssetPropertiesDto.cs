// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Web.Dtos
{
    public class AssetPropertiesDto
    {
        public int Id { get; set; }

        public double? AssetHeight { get; set; }

        public double? AssetWidth { get; set; }

        public double? AssetLength { get; set; }

        public double? AssetWeight { get; set; }

        public int? AssetAxels { get; set; }

        public int? AssetTrailers { get; set; }

        public bool? AssetSemi { get; set; }

        public double? AssetMaxGradient { get; set; }

        public double? AssetMinTurnRadius { get; set; }
    }
}
