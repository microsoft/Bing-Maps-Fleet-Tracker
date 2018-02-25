namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCircularGeofences : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoFences", "AreaType", c => c.Int(nullable: false));
            AddColumn("dbo.GeoFences", "Radius", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoFences", "Radius");
            DropColumn("dbo.GeoFences", "AreaType");
        }
    }
}
