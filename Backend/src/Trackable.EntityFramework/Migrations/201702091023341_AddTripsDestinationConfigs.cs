// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTripsDestinationConfigs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.Destinations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        Location = c.Geography(),
                        MinimumWaitTime = c.Int(),
                        InterestLevel = c.Int(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TripLegs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Route = c.Geography(),
                        AverageSpeed = c.Double(nullable: false),
                        TripDataId = c.Int(nullable: false),
                        StartDestinationId = c.Int(nullable: false),
                        EndDestinationId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Destinations", t => t.EndDestinationId)
                .ForeignKey("dbo.Destinations", t => t.StartDestinationId)
                .ForeignKey("dbo.Trips", t => t.TripDataId, cascadeDelete: true)
                .Index(t => t.TripDataId)
                .Index(t => t.StartDestinationId)
                .Index(t => t.EndDestinationId);
            
            AddColumn("dbo.Trips", "StartDestinationId", c => c.Int(nullable: false));
            AddColumn("dbo.Trips", "EndDestinationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Trips", "StartDestinationId");
            CreateIndex("dbo.Trips", "EndDestinationId");
            AddForeignKey("dbo.Trips", "EndDestinationId", "dbo.Destinations", "Id");
            AddForeignKey("dbo.Trips", "StartDestinationId", "dbo.Destinations", "Id");
            DropColumn("dbo.Trips", "Route");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trips", "Route", c => c.Geography());
            DropForeignKey("dbo.TripLegs", "TripDataId", "dbo.Trips");
            DropForeignKey("dbo.TripLegs", "StartDestinationId", "dbo.Destinations");
            DropForeignKey("dbo.TripLegs", "EndDestinationId", "dbo.Destinations");
            DropForeignKey("dbo.Trips", "StartDestinationId", "dbo.Destinations");
            DropForeignKey("dbo.Trips", "EndDestinationId", "dbo.Destinations");
            DropIndex("dbo.TripLegs", new[] { "EndDestinationId" });
            DropIndex("dbo.TripLegs", new[] { "StartDestinationId" });
            DropIndex("dbo.TripLegs", new[] { "TripDataId" });
            DropIndex("dbo.Trips", new[] { "EndDestinationId" });
            DropIndex("dbo.Trips", new[] { "StartDestinationId" });
            DropColumn("dbo.Trips", "EndDestinationId");
            DropColumn("dbo.Trips", "StartDestinationId");
            DropTable("dbo.TripLegs");
            DropTable("dbo.Destinations");
            DropTable("dbo.Configurations");
        }
    }
}
