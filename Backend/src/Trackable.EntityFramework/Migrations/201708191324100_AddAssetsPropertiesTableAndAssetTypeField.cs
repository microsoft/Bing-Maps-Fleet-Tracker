namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetsPropertiesTableAndAssetTypeField : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssetHeight = c.Double(),
                        AssetWidth = c.Double(),
                        AssetLength = c.Double(),
                        AssetWeight = c.Double(),
                        AssetAxels = c.Int(),
                        AssetTrailers = c.Int(),
                        AssetSemi = c.Boolean(),
                        AssetMaxGradient = c.Double(),
                        AssetMinTurnRadius = c.Double(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                        AssetId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .Index(t => t.AssetId);
            
            AddColumn("dbo.Assets", "AssetType", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetProperties", "AssetId", "dbo.Assets");
            DropIndex("dbo.AssetProperties", new[] { "AssetId" });
            DropColumn("dbo.Assets", "AssetType");
            DropTable("dbo.AssetProperties");
        }
    }
}
