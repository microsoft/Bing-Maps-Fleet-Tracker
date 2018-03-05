// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using System.Data.Entity.Spatial;

namespace Trackable.Repositories.Helpers
{
    public static class DbGeographyExtension
    {
        public static DbGeography MakeValid(this DbGeography geom)
        {
            return DbGeography.FromText(SqlGeometry.STGeomFromText(new SqlChars(geom.AsText()), 4326).MakeValid().STAsText().ToSqlString().ToString(), 4326);
        }
    }
}