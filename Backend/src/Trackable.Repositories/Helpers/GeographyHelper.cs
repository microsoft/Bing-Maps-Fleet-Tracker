using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;

namespace Trackable.Repositories.Helpers
{
    public static class GeographyHelper
    {
        //The id of the sphere mapping to Earth in SQL Server
        private const int EarthSRID = 4326;

        public static DbGeography CreateDbPoint(IPoint point)
        {
            return DbGeography.PointFromText(
                string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", point.Longitude, point.Latitude),
                EarthSRID);
        }

        public static DbGeography CreateDbLine(IEnumerable<IPoint> points)
        {
            var distinctPoints = points.Distinct(new PointEqualityComparer());
            var pointsString = String.Join(", ", distinctPoints.Select(p => string.Format(CultureInfo.InvariantCulture,
                "{0} {1}", p.Longitude, p.Latitude)));

            return DbGeography.LineFromText($"LINESTRING ({pointsString})", EarthSRID);
        }

        public static DbGeography CreatePolygon(IEnumerable<IPoint> points)
        {
            var pointsList = points.ToList();
            var equalityComparer = new PointEqualityComparer();

            if (!equalityComparer.Equals(pointsList.Last(), pointsList.First()))
            {
                pointsList.Add(pointsList.First());
            }

            var pointsString = String.Join(", ", pointsList.Select(p => string.Format(CultureInfo.InvariantCulture,
                "{0} {1}", p.Longitude, p.Latitude)));

            return DbGeography.PolygonFromText($"POLYGON (({pointsString}))", EarthSRID);
        }

        public static IEnumerable<Point> FromDbLine(DbGeography line)
        {
            for (int i = 1; i <= line.PointCount; i++)
            {
                var p = line.PointAt(i);
                yield return new Point() { Latitude = p.Latitude.Value, Longitude = p.Longitude.Value };
            }
        }

        public static IEnumerable<Point> FromPolygon(DbGeography polygon)
        {
            try
            {
                return FromPolygonInternal(polygon);
            }
            catch (Exception)
            {
                var validPolygon = polygon.MakeValid();
                return FromPolygonInternal(validPolygon);
            }
        }

        private static IEnumerable<Point> FromPolygonInternal(DbGeography polygon)
        {
            var list = new List<Point>();

            for (int i = 1; i <= polygon.PointCount; i++)
            {
                var p = polygon.PointAt(i);
                list.Add(new Point() { Latitude = p.Latitude.Value, Longitude = p.Longitude.Value });
            }

            return list;
        }
    }
}
