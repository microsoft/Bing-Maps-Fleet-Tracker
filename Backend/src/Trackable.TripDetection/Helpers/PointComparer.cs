// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories.Helpers
{
    /// <summary>
    /// Provides point comparison based on lat long
    /// </summary>
    public class PointComparer : IComparer<Point>
    {
        public int Compare(Point x, Point y)
        {
            if (x.Latitude < y.Latitude)
            {
                return -1;
            }

            if (x.Latitude == y.Latitude)
            {
                if (x.Longitude == y.Longitude)
                {
                    return 0;
                }

                if (x.Longitude < y.Longitude)
                {
                    return -1;
                }
            }

            return 1;
        }
    }

}
