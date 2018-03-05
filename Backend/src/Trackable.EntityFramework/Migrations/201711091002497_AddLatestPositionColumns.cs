// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLatestPositionColumns : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrackingPoints", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.TrackingPoints", "TrackingDeviceId", "dbo.TrackingDevices");
            AddColumn("dbo.Assets", "LatestPosition_Id", c => c.Int());
            AddColumn("dbo.TrackingDevices", "LatestPosition_Id", c => c.Int());
            CreateIndex("dbo.Assets", "LatestPosition_Id");
            CreateIndex("dbo.TrackingDevices", "LatestPosition_Id");
            AddForeignKey("dbo.TrackingDevices", "LatestPosition_Id", "dbo.TrackingPoints", "Id");
            AddForeignKey("dbo.Assets", "LatestPosition_Id", "dbo.TrackingPoints", "Id");
            AddForeignKey("dbo.TrackingPoints", "AssetId", "dbo.Assets", "Id");
            AddForeignKey("dbo.TrackingPoints", "TrackingDeviceId", "dbo.TrackingDevices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrackingPoints", "TrackingDeviceId", "dbo.TrackingDevices");
            DropForeignKey("dbo.TrackingPoints", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.Assets", "LatestPosition_Id", "dbo.TrackingPoints");
            DropForeignKey("dbo.TrackingDevices", "LatestPosition_Id", "dbo.TrackingPoints");
            DropIndex("dbo.TrackingDevices", new[] { "LatestPosition_Id" });
            DropIndex("dbo.Assets", new[] { "LatestPosition_Id" });
            DropColumn("dbo.TrackingDevices", "LatestPosition_Id");
            DropColumn("dbo.Assets", "LatestPosition_Id");
            AddForeignKey("dbo.TrackingPoints", "TrackingDeviceId", "dbo.TrackingDevices", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrackingPoints", "AssetId", "dbo.Assets", "Id", cascadeDelete: true);
        }
    }
}
