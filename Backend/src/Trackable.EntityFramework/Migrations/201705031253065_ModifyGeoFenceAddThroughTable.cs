namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyGeoFenceAddThroughTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.NotificationUpdates", newName: "GeoFenceUpdates");
            CreateTable(
                "dbo.GeoFenceDataAssetDatas",
                c => new
                    {
                        GeoFenceData_Id = c.Int(nullable: false),
                        AssetData_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GeoFenceData_Id, t.AssetData_Id })
                .ForeignKey("dbo.GeoFences", t => t.GeoFenceData_Id, cascadeDelete: true)
                .ForeignKey("dbo.Assets", t => t.AssetData_Id, cascadeDelete: true)
                .Index(t => t.GeoFenceData_Id)
                .Index(t => t.AssetData_Id);
            
            AddColumn("dbo.GeoFences", "Emails", c => c.String(nullable: false));
            AddColumn("dbo.GeoFences", "CooldownInMinutes", c => c.Long(nullable: false));
            DropColumn("dbo.GeoFences", "Email");
            DropColumn("dbo.GeoFences", "CooldownInSeconds");
            DropColumn("dbo.GeoFences", "AssetIds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GeoFences", "AssetIds", c => c.String(nullable: false));
            AddColumn("dbo.GeoFences", "CooldownInSeconds", c => c.Long(nullable: false));
            AddColumn("dbo.GeoFences", "Email", c => c.String(nullable: false));
            DropForeignKey("dbo.GeoFenceDataAssetDatas", "AssetData_Id", "dbo.Assets");
            DropForeignKey("dbo.GeoFenceDataAssetDatas", "GeoFenceData_Id", "dbo.GeoFences");
            DropIndex("dbo.GeoFenceDataAssetDatas", new[] { "AssetData_Id" });
            DropIndex("dbo.GeoFenceDataAssetDatas", new[] { "GeoFenceData_Id" });
            DropColumn("dbo.GeoFences", "CooldownInMinutes");
            DropColumn("dbo.GeoFences", "Emails");
            DropTable("dbo.GeoFenceDataAssetDatas");
            RenameTable(name: "dbo.GeoFenceUpdates", newName: "NotificationUpdates");
        }
    }
}
