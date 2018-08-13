namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAvoidsInDispatches : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dispatches", "Optimize", c => c.Int());
            DropColumn("dbo.Dispatches", "Avoid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Dispatches", "Avoid", c => c.String());
            AlterColumn("dbo.Dispatches", "Optimize", c => c.String());
        }
    }
}
