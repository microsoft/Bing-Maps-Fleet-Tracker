// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceIdToString : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", "dbo.GeoFences");
            DropForeignKey("dbo.GeoFenceUpdates", "GeoFenceDataId", "dbo.GeoFences");
        
            // DROP legacy named constraints
            Sql("ALTER TABLE \"GeoFenceUpdates\" DROP CONSTRAINT \"FK_dbo.NotificationUpdates_dbo.GeoFences_GeoFenceDataId\";");
            Sql("ALTER TABLE \"TripLegs\" DROP CONSTRAINT \"FK_dbo.TripLegs_dbo.Destinations_StartDestinationId\";");
            Sql("ALTER TABLE \"TripLegs\" DROP CONSTRAINT \"FK_dbo.TripLegs_dbo.Destinations_EndDestinationId\";");
            Sql("ALTER TABLE \"Trips\" DROP CONSTRAINT \"FK_dbo.Trips_dbo.Destinations_StartDestinationId\";");
            Sql("ALTER TABLE \"Trips\" DROP CONSTRAINT \"FK_dbo.Trips_dbo.Destinations_EndDestinationId\";");

            DropForeignKey("dbo.Tags", "GeoFenceData_Id", "dbo.GeoFences");
            DropForeignKey("dbo.Tags", "LocationData_Id", "dbo.Locations");
            DropForeignKey("dbo.Trips", "EndLocationId", "dbo.Locations");
            DropForeignKey("dbo.Trips", "StartLocationId", "dbo.Locations");
            DropForeignKey("dbo.TripLegs", "EndLocationId", "dbo.Locations");
            DropForeignKey("dbo.TripLegs", "StartLocationId", "dbo.Locations");
            DropForeignKey("dbo.Tokens", "User_Id", "dbo.Users");
            DropIndex("dbo.GeoFenceUpdates", "GeoFenceAssetUnique");
            DropIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId" });
            DropIndex("dbo.Tags", new[] { "GeoFenceData_Id" });
            DropIndex("dbo.Tags", new[] { "LocationData_Id" });
            DropIndex("dbo.Trips", new[] { "StartLocationId" });
            DropIndex("dbo.Trips", new[] { "EndLocationId" });
            DropIndex("dbo.TripLegs", new[] { "StartLocationId" });
            DropIndex("dbo.TripLegs", new[] { "EndLocationId" });
            DropIndex("dbo.Tokens", new[] { "User_Id" });
            DropIndex("dbo.GeoFenceDataAssetDatas", new[] { "GeoFenceData_Id" });
            DropPrimaryKey("dbo.GeoFences");
            DropPrimaryKey("dbo.Locations");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.GeoFenceDataAssetDatas");
            AddColumn("dbo.Assets", "Name", c => c.String());

            // ID Column migration
            RenameColumn("dbo.GeoFences", "Id", "PrevId");
            RenameColumn("dbo.Locations", "Id", "PrevId");
            RenameColumn("dbo.Users", "Id", "PrevId");

            AddColumn("dbo.GeoFences", "Id", c => c.String(nullable: true, maxLength: 128));
            AddColumn("dbo.Locations", "Id", c => c.String(nullable: true, maxLength: 128));
            AddColumn("dbo.Users", "Id", c => c.String(nullable: true, maxLength: 128));
            
            Sql("Update \"GeoFences\" set Id = PrevId;");
            Sql("Update \"Locations\" set Id = PrevId;");
            Sql("Update \"Users\" set Id = PrevId;");
            Sql("Update \"Assets\" set Name = Id;");

            AlterColumn("dbo.GeoFences", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Locations", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Users", "Id", c => c.String(nullable: false, maxLength: 128));

            DropColumn("dbo.GeoFences", "PrevId");
            DropColumn("dbo.Locations", "PrevId");
            DropColumn("dbo.Users", "PrevId");
            // End ID Column migration

            AlterColumn("dbo.GeoFenceUpdates", "GeoFenceDataId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Tags", "GeoFenceData_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Tags", "LocationData_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Trips", "StartLocationId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Trips", "EndLocationId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.TripLegs", "StartLocationId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.TripLegs", "EndLocationId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Tokens", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.GeoFences", "Id");
            AddPrimaryKey("dbo.Locations", "Id");
            AddPrimaryKey("dbo.Users", "Id");
            AddPrimaryKey("dbo.GeoFenceDataAssetDatas", new[] { "GeoFenceData_Id", "AssetData_Id" });
            CreateIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId", "AssetDataId" }, unique: true, name: "GeoFenceAssetUnique");
            CreateIndex("dbo.GeoFenceUpdates", "GeoFenceDataId");
            CreateIndex("dbo.Tags", "GeoFenceData_Id");
            CreateIndex("dbo.Tags", "LocationData_Id");
            CreateIndex("dbo.Tokens", "User_Id");
            CreateIndex("dbo.Trips", "StartLocationId");
            CreateIndex("dbo.Trips", "EndLocationId");
            CreateIndex("dbo.TripLegs", "StartLocationId");
            CreateIndex("dbo.TripLegs", "EndLocationId");
            CreateIndex("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id");
            AddForeignKey("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", "dbo.GeoFences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GeoFenceUpdates", "GeoFenceDataId", "dbo.GeoFences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tags", "GeoFenceData_Id", "dbo.GeoFences", "Id");
            AddForeignKey("dbo.Tags", "LocationData_Id", "dbo.Locations", "Id");
            AddForeignKey("dbo.Trips", "EndLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Trips", "StartLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.TripLegs", "EndLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.TripLegs", "StartLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Tokens", "User_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.TripLegs", "StartLocationId", "dbo.Locations");
            DropForeignKey("dbo.TripLegs", "EndLocationId", "dbo.Locations");
            DropForeignKey("dbo.Trips", "StartLocationId", "dbo.Locations");
            DropForeignKey("dbo.Trips", "EndLocationId", "dbo.Locations");
            DropForeignKey("dbo.Tags", "LocationData_Id", "dbo.Locations");
            DropForeignKey("dbo.Tags", "GeoFenceData_Id", "dbo.GeoFences");
            DropForeignKey("dbo.GeoFenceUpdates", "GeoFenceDataId", "dbo.GeoFences");
            DropForeignKey("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", "dbo.GeoFences");
            DropIndex("dbo.GeoFenceDataAssetDatas", new[] { "GeoFenceData_Id" });
            DropIndex("dbo.TripLegs", new[] { "EndLocationId" });
            DropIndex("dbo.TripLegs", new[] { "StartLocationId" });
            DropIndex("dbo.Trips", new[] { "EndLocationId" });
            DropIndex("dbo.Trips", new[] { "StartLocationId" });
            DropIndex("dbo.Tokens", new[] { "User_Id" });
            DropIndex("dbo.Tags", new[] { "LocationData_Id" });
            DropIndex("dbo.Tags", new[] { "GeoFenceData_Id" });
            DropIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId" });
            DropIndex("dbo.GeoFenceUpdates", "GeoFenceAssetUnique");
            DropPrimaryKey("dbo.GeoFenceDataAssetDatas");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.Locations");
            DropPrimaryKey("dbo.GeoFences");
            AlterColumn("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Tokens", "User_Id", c => c.Guid());
            AlterColumn("dbo.Users", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.TripLegs", "EndLocationId", c => c.Int(nullable: false));
            AlterColumn("dbo.TripLegs", "StartLocationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Locations", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Trips", "EndLocationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Trips", "StartLocationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tags", "LocationData_Id", c => c.Int());
            AlterColumn("dbo.Tags", "GeoFenceData_Id", c => c.Int());
            AlterColumn("dbo.GeoFenceUpdates", "GeoFenceDataId", c => c.Int(nullable: false));
            AlterColumn("dbo.GeoFences", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Assets", "Name");
            AddPrimaryKey("dbo.GeoFenceDataAssetDatas", new[] { "GeoFenceData_Id", "AssetData_Id" });
            AddPrimaryKey("dbo.Users", "Id");
            AddPrimaryKey("dbo.Locations", "Id");
            AddPrimaryKey("dbo.GeoFences", "Id");
            CreateIndex("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id");
            CreateIndex("dbo.Tokens", "User_Id");
            CreateIndex("dbo.TripLegs", "EndLocationId");
            CreateIndex("dbo.TripLegs", "StartLocationId");
            CreateIndex("dbo.Trips", "EndLocationId");
            CreateIndex("dbo.Trips", "StartLocationId");
            CreateIndex("dbo.Tags", "LocationData_Id");
            CreateIndex("dbo.Tags", "GeoFenceData_Id");
            CreateIndex("dbo.GeoFenceUpdates", "GeoFenceDataId");
            CreateIndex("dbo.GeoFenceUpdates", new[] { "GeoFenceDataId", "AssetDataId" }, unique: true, name: "GeoFenceAssetUnique");
            AddForeignKey("dbo.Tokens", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.TripLegs", "StartLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.TripLegs", "EndLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Trips", "StartLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Trips", "EndLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Tags", "LocationData_Id", "dbo.Locations", "Id");
            AddForeignKey("dbo.Tags", "GeoFenceData_Id", "dbo.GeoFences", "Id");
            AddForeignKey("dbo.GeoFenceUpdates", "GeoFenceDataId", "dbo.GeoFences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", "dbo.GeoFences", "Id", cascadeDelete: true);
        }
    }
}
