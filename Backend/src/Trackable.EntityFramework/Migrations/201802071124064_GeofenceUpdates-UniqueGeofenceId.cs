// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeofenceUpdatesUniqueGeofenceId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId" });
            DropIndex("dbo.GeoFenceUpdates", new[] { "AssetDataId" });

            //For uniqueness constraint to pass
            Sql(@"
            DELETE FROM GeoFenceUpdates
                WHERE Id <> (SELECT max(Id) 
                FROM GeofenceUpdates g2
                WHERE g2.GeoFenceDataId  = GeoFenceUpdates.GeoFenceDataId and g2.AssetDataId  = GeoFenceUpdates.AssetDataId)");

            CreateIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId", "AssetDataId" }, unique: true, name: "GeoFenceAssetUnique");
            CreateIndex("dbo.GeoFenceUpdates", "GeoFenceDataId");
            CreateIndex("dbo.GeoFenceUpdates", "AssetDataId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.GeoFenceUpdates", new[] { "AssetDataId" });
            DropIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId" });
            DropIndex("dbo.GeoFenceUpdates", "GeoFenceAssetUnique");
            CreateIndex("dbo.GeoFenceUpdates", "AssetDataId");
            CreateIndex("dbo.GeoFenceUpdates", "GeoFenceDataId");
        }
    }
}
