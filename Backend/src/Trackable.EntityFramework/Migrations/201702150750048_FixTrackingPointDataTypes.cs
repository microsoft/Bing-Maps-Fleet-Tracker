// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrackingPointDataTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackingPoints", "LocationProvider", c => c.Int(nullable: false));
            AlterColumn("dbo.TrackingPoints", "Accuracy", c => c.Double());
            AlterColumn("dbo.TrackingPoints", "Speed", c => c.Double());
            AlterColumn("dbo.TrackingPoints", "Altitude", c => c.Double());
            AlterColumn("dbo.TrackingPoints", "Bearing", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrackingPoints", "Bearing", c => c.Int());
            AlterColumn("dbo.TrackingPoints", "Altitude", c => c.Int());
            AlterColumn("dbo.TrackingPoints", "Speed", c => c.Int());
            AlterColumn("dbo.TrackingPoints", "Accuracy", c => c.Int());
            DropColumn("dbo.TrackingPoints", "LocationProvider");
        }
    }
}
