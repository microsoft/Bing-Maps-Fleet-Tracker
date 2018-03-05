// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDateTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "StartTimeUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Trips", "EndTimeUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.TripLegs", "StartTimeUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.TripLegs", "EndTimeUtc", c => c.DateTime(nullable: false));
            DropColumn("dbo.Trips", "StartTime");
            DropColumn("dbo.Trips", "EndTime");
            DropColumn("dbo.TripLegs", "StartTime");
            DropColumn("dbo.TripLegs", "EndTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TripLegs", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.TripLegs", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Trips", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Trips", "StartTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.TripLegs", "EndTimeUtc");
            DropColumn("dbo.TripLegs", "StartTimeUtc");
            DropColumn("dbo.Trips", "EndTimeUtc");
            DropColumn("dbo.Trips", "StartTimeUtc");
        }
    }
}
