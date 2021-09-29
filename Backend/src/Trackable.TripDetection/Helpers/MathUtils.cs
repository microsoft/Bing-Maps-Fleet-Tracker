// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using Trackable.Common;
using Trackable.Models;

namespace Trackable.TripDetection.Helpers
{
    internal static class MathUtils
    {
        /// <summary>
        /// Calculates average speed of a set of points
        /// </summary>
        /// <param name="points">Array of points</param>
        /// <returns>The average speed in meters per second or double.Max if there is no
        /// difference in time between the points.</returns>
        public static double AverageSpeed(params TrackingPoint[] points)
        {
            if (points.Length < 2)
            {
                return 0;
            }

            double distance = 0;
            long startTime = long.MaxValue;
            long endTime = long.MinValue;

            for (var i = 0; i < points.Length - 1; i++)
            {
                distance += DistanceInMeters(points[i], points[i + 1]);

                if (startTime > points[i].DeviceTimestampUtc)
                {
                    startTime = points[i].DeviceTimestampUtc;
                }

                if (endTime < points[i].DeviceTimestampUtc)
                {
                    endTime = points[i].DeviceTimestampUtc;
                }
            }

            if (startTime > points.Last().DeviceTimestampUtc)
            {
                startTime = points.Last().DeviceTimestampUtc;
            }

            if (endTime < points.Last().DeviceTimestampUtc)
            {
                endTime = points.Last().DeviceTimestampUtc;
            }

            var timeInMilliseconds = DateTimeUtils.DifferenceInMilliseconds(startTime, endTime);

            if (timeInMilliseconds <= 0)
            {
                return distance == 0 ? 0 : double.MaxValue;
            }

            return 1000 * distance / timeInMilliseconds;
        }

        /// <summary>
        /// Calculates average acceleration of a set of points
        /// </summary>
        /// <param name="points">Array of points</param>
        /// <returns>The average acceleration in meters per second^2 or double.Max if there is no
        /// difference in time between the points.</returns>
        public static double AverageAcceleration(params SpeedAtTime[] speeds)
        {
            if (speeds.Length < 2)
            {
                return 0;
            }

            long startTime = long.MaxValue;
            long endTime = long.MinValue;
            double speed = 0;

            for (var i = 0; i < speeds.Length - 1; i++)
            {
                speed += Math.Abs(speeds[i].Speed - speeds[i + 1].Speed);

                if (startTime > speeds[i].TimestampUtc)
                {
                    startTime = speeds[i].TimestampUtc;
                }

                if (endTime < speeds[i].TimestampUtc)
                {
                    endTime = speeds[i].TimestampUtc;
                }
            }

            if (startTime > speeds.Last().TimestampUtc)
            {
                startTime = speeds.Last().TimestampUtc;
            }

            if (endTime < speeds.Last().TimestampUtc)
            {
                endTime = speeds.Last().TimestampUtc;
            }

            var timeInMilliseconds = DateTimeUtils.DifferenceInMilliseconds(startTime, endTime);

            if (timeInMilliseconds <= 0 )
            {
                return speed == 0 ? 0 : double.MaxValue;
            }

            return 1000 * speed / timeInMilliseconds;
        }

        /// <summary>
        /// Calculates the angle between two points
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Seconds point</param>
        /// <returns>Angle between p1 and p2 in range [0, 2 * Pi)</returns>
        public static double AngleBetweenPoints(IPoint p1, IPoint p2)
        {
            var angle = Math.Atan2(p1.Latitude - p2.Latitude, p1.Longitude - p2.Longitude);

            if (angle < 0)
            {
                angle += Math.PI * 2;
            }

            return angle;
        }

        /// <summary>
        /// Calculates the distance in meters between two geo points
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>The distance between the points in meters</returns>
        public static double DistanceInMeters(IPoint point1, IPoint point2)
        {
            var startCoord = new GeoCoordinate(point1.Latitude, point1.Longitude);
            var endCoord = new GeoCoordinate(point2.Latitude, point2.Longitude);

            return startCoord.GetDistanceTo(endCoord);
        }

        /// <summary>
        /// Gets the center of mass of a list of points
        /// </summary>
        /// <param name="points">list of points</param>
        /// <returns>Center of mass as a new point</returns>
        public static IPoint CenterOfMass(IEnumerable<IPoint> points)
        {
            double longitudeCenter = 0, latitudeCenter = 0;
            foreach (var point in points)
            {
                longitudeCenter += point.Longitude;
                latitudeCenter += point.Latitude;
            }

            longitudeCenter /= points.Count();
            latitudeCenter /= points.Count();

            return new Point(latitudeCenter, longitudeCenter);
        }

        /// <summary>
        /// Calculates the average speed of each point compared to the point before and after
        /// </summary>
        /// <param name="points">List of points to calculate speed for</param>
        /// <returns>Struct containing the speed corresponding to each point, and the timestamp of that point</returns>
        public static IList<SpeedAtTime> AveragePointSpeeds(IList<TrackingPoint> points)
        {
            var instantaneousSpeeds = new List<SpeedAtTime>(points.Count);

            instantaneousSpeeds.Add(
                new SpeedAtTime(
                    AverageSpeed(points.First(), points[1]),
                    points.First().DeviceTimestampUtc));

            for (var i = 1; i < points.Count - 1; i++)
            {
                instantaneousSpeeds.Add(
                    new SpeedAtTime(
                        AverageSpeed(points[i - 1], points[i], points[i + 1]),
                        points[i].DeviceTimestampUtc));
            }

            instantaneousSpeeds.Add(
                new SpeedAtTime(
                    AverageSpeed(points.Last(), points[points.Count - 1]),
                    points.Last().DeviceTimestampUtc));

            return instantaneousSpeeds;
        }

        /// <summary>
        /// Calculates the average acceleration of each point compared to the point before and after
        /// </summary>
        /// <param name="points">List of points to calculate acceleration for</param>
        /// <returns>Acceleration corresponding to each point</returns>
        public static IList<double> AveragePointAccelerations(IList<TrackingPoint> points)
        {
            var speeds = AveragePointSpeeds(points);
            var accelerations = new List<double>(points.Count);

            accelerations.Add(AverageAcceleration(speeds.First(), speeds[1]));

            for (int i = 1; i < points.Count - 1; i++)
            {
                accelerations.Add(AverageAcceleration(speeds[i - 1], speeds[i], speeds[i + 1]));
            }

            accelerations.Add(AverageAcceleration(speeds.Last(), speeds[points.Count - 1]));

            return accelerations;
        }

        /// <summary>
        /// Approximate contour area of a polygon
        /// </summary>
        /// <param name="points">Points representing polygon</param>
        /// <returns>Measure indicating area</returns>
        public static double ApproximateContourArea(IEnumerable<IPoint> points)
        {
            // Add first point to the end as polygons are cyclic
            var clonedPoints = new List<IPoint>(points);
            clonedPoints.Add(clonedPoints[0]);

            double sum = 0;
            for (int i = 0; i < clonedPoints.Count - 1; i++)
            {
                sum += (clonedPoints[i + 1].Latitude - clonedPoints[i].Latitude) * (clonedPoints[i + 1].Longitude + clonedPoints[i].Longitude);
            }

            return sum / 2;
        }

        // Simple approximation for minimium bounding circle based on Ritter's bounding sphere
        // https://en.wikipedia.org/wiki/Bounding_sphere
        public static void BoundingCircle(IEnumerable<IPoint> points, out IPoint centerPoint, out double radius)
        {
            var seedPoint = points.First();

            var furthestPoint = points.OrderBy(
               labeledPoint => labeledPoint,
               new PivotDistanceComparer(seedPoint))
               .Last();

            var furthestFromFurthest = points.OrderBy(
               labeledPoint => labeledPoint,
               new PivotDistanceComparer(furthestPoint))
               .Last();

            centerPoint = CenterOfMass(new[] { furthestPoint, furthestFromFurthest });
            radius = DistanceInMeters(furthestPoint, furthestFromFurthest) / 2;
        }

        /// <summary>
        /// Loops over supplied points and smooths through the use of window averaging
        /// </summary>
        /// <param name="windowInput">List of points to be smoothed</param>
        /// <param name="smoothingWindowSize">Averagin window size</param>
        public static void SmoothPoints(IList<TrackingPoint> points, int smoothingWindowSize)
        {
            var windowInput = new List<TrackingPoint>(points);

            // If points are fewer than the size of the window, then we cant smooth
            if (windowInput.Count < smoothingWindowSize)
            {
                return;
            }

            // Extend the points array on both ends by windowSize / 2 to allow filter to process edge elements
            for (int i = 0; i < smoothingWindowSize / 2; i++)
            {
                windowInput.Insert(0, windowInput.First());
                windowInput.Add(windowInput.Last());
            }

            // Calculate the average window longitude and latitude and assign the point these values
            double latitude;
            double longitude;
            for (int i = 0; i < windowInput.Count - smoothingWindowSize; i++)
            {
                latitude = 0;
                longitude = 0;
                for (int j = 0; j < smoothingWindowSize; j++)
                {
                    IPoint p = windowInput[i + j];
                    latitude += p.Latitude;
                    longitude += p.Longitude;
                }

                var middlePoint = windowInput[i + smoothingWindowSize / 2];
                middlePoint.Latitude = latitude / smoothingWindowSize;
                middlePoint.Longitude = longitude / smoothingWindowSize;
            }
        }

        /// <summary>
        /// Calculates area of a circle
        /// </summary>
        /// <param name="radius">Radius of circle</param>
        /// <returns>Area of circle</returns>
        public static double CircularArea(double radius)
        {
            return Math.PI * radius * radius;
        }

        internal struct SpeedAtTime
        {
            public double Speed { get; }
            public long TimestampUtc { get; }

            public SpeedAtTime(double speed, long time)
            {
                this.Speed = speed;
                this.TimestampUtc = time;
            }
        }
    }
}
