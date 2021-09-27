// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Trackable.Models;

namespace Trackable.Repositories.Helpers
{
    /// <summary>
    /// Provides equality check for two points
    /// </summary>
    public class PointEqualityComparer : IEqualityComparer<IPoint>
    {
        public bool Equals(IPoint x, IPoint y)
        {
            return x.Latitude == y.Latitude
                && x.Longitude == y.Longitude;
        }

        public int GetHashCode(IPoint obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Latitude.GetHashCode();
                hash = hash * 23 + obj.Longitude.GetHashCode();
                return hash;
            }
        }
    }

}
