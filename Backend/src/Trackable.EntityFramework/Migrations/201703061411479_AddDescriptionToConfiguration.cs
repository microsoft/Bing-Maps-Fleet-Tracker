namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToConfiguration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Configurations", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Configurations", "Description");
        }
    }
}
