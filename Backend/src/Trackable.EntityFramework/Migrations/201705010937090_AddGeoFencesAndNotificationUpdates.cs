namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeoFencesAndNotificationUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationUpdates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GeoFenceDataId = c.Int(nullable: false),
                        AssetDataId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetDataId)
                .ForeignKey("dbo.GeoFences", t => t.GeoFenceDataId, cascadeDelete: true)
                .Index(t => t.GeoFenceDataId)
                .Index(t => t.AssetDataId);
            
            CreateTable(
                "dbo.GeoFences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        FenceType = c.Int(nullable: false),
                        CooldownInSeconds = c.Long(nullable: false),
                        Polygon = c.Geography(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationUpdates", "GeoFenceDataId", "dbo.GeoFences");
            DropForeignKey("dbo.NotificationUpdates", "AssetDataId", "dbo.Assets");
            DropIndex("dbo.NotificationUpdates", new[] { "AssetDataId" });
            DropIndex("dbo.NotificationUpdates", new[] { "GeoFenceDataId" });
            DropTable("dbo.GeoFences");
            DropTable("dbo.NotificationUpdates");
        }
    }
}
