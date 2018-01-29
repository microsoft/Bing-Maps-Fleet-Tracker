namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrackingDevices",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Model = c.String(),
                        Phone = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                        AssetId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.TrackingPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceTimeUtc = c.DateTime(nullable: false),
                        Location = c.Geography(),
                        Provider = c.String(),
                        Debug = c.Boolean(nullable: false),
                        Accuracy = c.Int(),
                        Speed = c.Int(),
                        Altitude = c.Int(),
                        Bearing = c.Int(),
                        TripId = c.Int(),
                        AssetId = c.String(nullable: false, maxLength: 128),
                        TrackingDeviceId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.TrackingDevices", t => t.TrackingDeviceId, cascadeDelete: true)
                .ForeignKey("dbo.Trips", t => t.TripId)
                .Index(t => t.TripId)
                .Index(t => t.AssetId)
                .Index(t => t.TrackingDeviceId);
            
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Route = c.Geography(),
                        AssetId = c.String(nullable: false, maxLength: 128),
                        TrackingDeviceId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.TrackingDevices", t => t.TrackingDeviceId, cascadeDelete: true)
                .Index(t => t.AssetId)
                .Index(t => t.TrackingDeviceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trips", "TrackingDeviceId", "dbo.TrackingDevices");
            DropForeignKey("dbo.TrackingPoints", "TripId", "dbo.Trips");
            DropForeignKey("dbo.Trips", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.TrackingPoints", "TrackingDeviceId", "dbo.TrackingDevices");
            DropForeignKey("dbo.TrackingPoints", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.TrackingDevices", "AssetId", "dbo.Assets");
            DropIndex("dbo.Trips", new[] { "TrackingDeviceId" });
            DropIndex("dbo.Trips", new[] { "AssetId" });
            DropIndex("dbo.TrackingPoints", new[] { "TrackingDeviceId" });
            DropIndex("dbo.TrackingPoints", new[] { "AssetId" });
            DropIndex("dbo.TrackingPoints", new[] { "TripId" });
            DropIndex("dbo.TrackingDevices", new[] { "AssetId" });
            DropTable("dbo.Trips");
            DropTable("dbo.TrackingPoints");
            DropTable("dbo.TrackingDevices");
            DropTable("dbo.Assets");
        }
    }
}
