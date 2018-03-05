// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Trackable.Models
{
    public class PolygonGeoFenceArea : IGeoFenceArea
    {
        public IEnumerable<Point> FencePolygon { get; set; }

        public GeoFenceAreaType AreaType => GeoFenceAreaType.Polygon;
    }
}
