// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Trackable.Common;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    [Serializable]
    internal abstract class TripSegmentBase
    {
        public IList<TrackingPoint> Points { get; set; }

        public abstract bool IsMovingSegment { get; }

        public TripSegmentBase()
        {
            this.Points = new List<TrackingPoint>();
        }

        public TripSegmentBase(IList<TrackingPoint> points)
        {
            this.Points = points;
        }

        public long GetDurationInSeconds()
        {
            if (this.Points.Count < 2)
            {
                return 0;
            }

            return DateTimeUtils.DifferenceInMilliseconds(
                this.Points.First().DeviceTimestampUtc,
                this.Points.Last().DeviceTimestampUtc)/ 1000;
        }

        public double GetAverageSpeed()
        {
            if (this.Points.Count < 2)
            {
                return 0;
            }

            var resultantDistance = GetBoundingRadius() * 2;
            var duration = GetDurationInSeconds();

            return resultantDistance / duration;
        }

        public double GetBoundingRadius()
        {
            if (this.Points.Count < 2)
            {
                return 0;
            }

            double radius;
            IPoint point;

            MathUtils.BoundingCircle(this.Points, out point, out radius);

            return radius; 
        }

        public IPoint GetGeometricalCenter()
        {
            if (this.Points.Count < 2)
            {
                return this.Points.FirstOrDefault();
            }

            double radius;
            IPoint point;

            MathUtils.BoundingCircle(this.Points, out point, out radius);

            return point;
        }

        public TrackingPoint GetMedianPoint()
        {
            if (!this.Points.Any())
            {
                return null;
            }

            var center = MathUtils.CenterOfMass(this.Points);

            var sortedPoints = this.Points.OrderBy(
                labeledPoint => labeledPoint,
                new PivotDistanceComparer(center));

            return sortedPoints.First();
        }

        public double GetCircularity()
        {
            if(this.Points.Count < 3)
            {
                return 0;
            }

            double radius;
            IPoint centerPoint;

            MathUtils.BoundingCircle(this.Points, out centerPoint, out radius);

            var contourArea = MathUtils.ApproximateContourArea(this.Points);
            var circularHullArea = MathUtils.CircularArea(radius);

            return contourArea / circularHullArea;
        }
    }
}
