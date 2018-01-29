namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToGeoFenceMakeNonNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NotificationUpdates", "AssetDataId", "dbo.Assets");
            DropIndex("dbo.NotificationUpdates", new[] { "AssetDataId" });
            AddColumn("dbo.GeoFences", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.NotificationUpdates", "AssetDataId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.GeoFences", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.GeoFences", "AssetIds", c => c.String(nullable: false));
            CreateIndex("dbo.NotificationUpdates", "AssetDataId");
            AddForeignKey("dbo.NotificationUpdates", "AssetDataId", "dbo.Assets", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationUpdates", "AssetDataId", "dbo.Assets");
            DropIndex("dbo.NotificationUpdates", new[] { "AssetDataId" });
            AlterColumn("dbo.GeoFences", "AssetIds", c => c.String());
            AlterColumn("dbo.GeoFences", "Email", c => c.String());
            AlterColumn("dbo.NotificationUpdates", "AssetDataId", c => c.String(maxLength: 128));
            DropColumn("dbo.GeoFences", "Name");
            CreateIndex("dbo.NotificationUpdates", "AssetDataId");
            AddForeignKey("dbo.NotificationUpdates", "AssetDataId", "dbo.Assets", "Id");
        }
    }
}
