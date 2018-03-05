// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.EntityFramework
{
    [Table("AssetProperties")]
    public class AssetPropertiesData: EntityBase<int>
    {
        public double? AssetHeight { get; set; }

        public double? AssetWidth { get; set; }

        public double? AssetLength { get; set; }

        public double? AssetWeight { get; set; }

        public int? AssetAxels { get; set; }

        public int? AssetTrailers { get; set; }

        public bool? AssetSemi { get; set; }

        public double? AssetMaxGradient { get; set; }

        public double? AssetMinTurnRadius { get; set; }

        public AssetData Asset { get; set; }
    }
}
