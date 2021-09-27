// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Models
{
    public class Point : IPoint
    {
        public Point()
        {
        }

        public Point(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public object Clone()
        {
            return new Point
            {
                Latitude = this.Latitude,
                Longitude = this.Longitude
            };
        }
    }
}
