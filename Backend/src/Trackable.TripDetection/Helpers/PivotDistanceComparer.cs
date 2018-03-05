// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    /// <summary>
    /// Comparator that ranks points based on their proximity to a specified point
    /// </summary>
    public class PivotDistanceComparer : IComparer<IPoint>
    {
        public IPoint pivot { get; set; }

        public PivotDistanceComparer(IPoint pivot)
        {
            this.pivot = pivot;
        }

        public int Compare(IPoint a, IPoint b)
        {
            var distanceA = MathUtils.DistanceInMeters(a, this.pivot);
            var distanceB = MathUtils.DistanceInMeters(b, this.pivot);

            if (distanceA > distanceB)
            {
                return 1;
            }

            if (distanceA < distanceB)
            {
                return -1;
            }

            return 0;
        }
    }
}
