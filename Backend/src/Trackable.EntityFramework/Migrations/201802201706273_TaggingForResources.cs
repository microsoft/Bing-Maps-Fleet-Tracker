// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaggingForResources : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(maxLength: 250, unicode: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                        GeoFenceData_Id = c.Int(),
                        TrackingDeviceData_Id = c.String(maxLength: 128),
                        LocationData_Id = c.Int(),
                        AssetData_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeoFences", t => t.GeoFenceData_Id)
                .ForeignKey("dbo.TrackingDevices", t => t.TrackingDeviceData_Id)
                .ForeignKey("dbo.Locations", t => t.LocationData_Id)
                .ForeignKey("dbo.Assets", t => t.AssetData_Id)
                .Index(t => t.GeoFenceData_Id)
                .Index(t => t.TrackingDeviceData_Id)
                .Index(t => t.LocationData_Id)
                .Index(t => t.AssetData_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "AssetData_Id", "dbo.Assets");
            DropForeignKey("dbo.Tags", "LocationData_Id", "dbo.Locations");
            DropForeignKey("dbo.Tags", "TrackingDeviceData_Id", "dbo.TrackingDevices");
            DropForeignKey("dbo.Tags", "GeoFenceData_Id", "dbo.GeoFences");
            DropIndex("dbo.Tags", new[] { "AssetData_Id" });
            DropIndex("dbo.Tags", new[] { "LocationData_Id" });
            DropIndex("dbo.Tags", new[] { "TrackingDeviceData_Id" });
            DropIndex("dbo.Tags", new[] { "GeoFenceData_Id" });
            DropTable("dbo.Tags");
        }
    }
}
