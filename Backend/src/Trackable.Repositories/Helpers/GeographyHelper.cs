using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
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

        public static DbGeography CreateDbMultiPoint(IPoint[] points)
        {
            var distinctPoints = points.Distinct(new PointEqualityComparer());
            var pointsString = String.Join(", ", distinctPoints.Select(p => string.Format(CultureInfo.InvariantCulture,
                "({0} {1})", p.Longitude, p.Latitude)));


            return DbGeography.MultiPointFromText(
                string.Format(CultureInfo.InvariantCulture, "MULTIPOINT({0})", pointsString),
                EarthSRID);
        }

        public static DbGeography CreateDbLine(IEnumerable<IPoint> points)
        {
            var distinctPoints = points.Distinct(new PointEqualityComparer());
            var pointsString = String.Join(", ", distinctPoints.Select(p => string.Format(CultureInfo.InvariantCulture,
                "{0} {1}", p.Longitude, p.Latitude)));

            return DbGeography.LineFromText($"LINESTRING ({pointsString})", EarthSRID);
        }

        public static DbGeography CreateDbPolygon(IEnumerable<IPoint> points)
        {
            var listPoints = points.ToList();

            if (!new PointEqualityComparer().Equals(listPoints.Last(), listPoints.First()))
            {
                listPoints.Add(listPoints.First());
            }

            var pointsString = String.Join(", ", listPoints.Select(p => string.Format(CultureInfo.InvariantCulture,
                "{0} {1}", p.Longitude, p.Latitude)));

            var wktString = $"POLYGON (({pointsString}))";

            var sqlGeography = SqlGeography.STGeomFromText(new SqlChars(wktString), EarthSRID).MakeValid();

            // SQL Server is left-handed when it comes to polygons
            // Bing Maps Control is ambidextrous
            var invertedSqlGeography = sqlGeography.ReorientObject();
            if (sqlGeography.STArea() > invertedSqlGeography.STArea())
            {
                sqlGeography = invertedSqlGeography;
            }

            return DbSpatialServices.Default.GeographyFromProviderValue(sqlGeography);
        }

        public static IEnumerable<Point> FromDbLine(DbGeography line)
        {
            for (int i = 1; i <= line.PointCount; i++)
            {
                var p = line.PointAt(i);
                yield return new Point() { Latitude = p.Latitude.Value, Longitude = p.Longitude.Value };
            }
        }

        public static IEnumerable<Point> FromDbPolygon(DbGeography polygon)
        {
            var sqlGeography = SqlGeography.STGeomFromText(new SqlChars(polygon.WellKnownValue.WellKnownText), EarthSRID);

            var list = new List<Point>();

            for (int i = 1; i <= polygon.PointCount; i++)
            {
                var p = sqlGeography.STPointN(i);
                list.Add(new Point() { Latitude = p.Lat.Value, Longitude = p.Long.Value });
            }

            return list;
        }
    }
}
