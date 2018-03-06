// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Web.Dtos
{
    public class AssetPropertiesDto
    {
        /// <summary>
        /// The height of the asset
        /// </summary>
        public double? AssetHeight { get; set; }
        
        /// <summary>
        /// The width of the asset
        /// </summary>
        public double? AssetWidth { get; set; }

        /// <summary>
        /// The length of the asset
        /// </summary>
        public double? AssetLength { get; set; }

        /// <summary>
        /// The weight of the asset
        /// </summary>
        public double? AssetWeight { get; set; }

        /// <summary>
        /// The number of axles the asset has
        /// </summary>
        public int? AssetAxels { get; set; }

        /// <summary>
        /// The number of trailers attached to the asset
        /// </summary>
        public int? AssetTrailers { get; set; }

        /// <summary>
        /// Asset is a semi truck
        /// </summary>
        public bool? AssetSemi { get; set; }

        /// <summary>
        /// Maximum gradient of inclination the asset can safely navigate
        /// </summary>
        public double? AssetMaxGradient { get; set; }

        /// <summary>
        /// The minimum turn radius the asset can use
        /// </summary>
        public double? AssetMinTurnRadius { get; set; }
    }
}
