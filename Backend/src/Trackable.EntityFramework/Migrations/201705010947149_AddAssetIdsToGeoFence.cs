namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetIdsToGeoFence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoFences", "AssetIds", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoFences", "AssetIds");
        }
    }
}
