namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyDispatchingPropertiesMakingNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DispatchPoints", "DispatchingData_Id", "dbo.Dispatching");
            DropIndex("dbo.DispatchPoints", new[] { "DispatchingData_Id" });
            AddColumn("dbo.DispatchPoints", "DispatchingData_Id1", c => c.Int());
            AlterColumn("dbo.Dispatching", "LoadedHeight", c => c.Double());
            AlterColumn("dbo.Dispatching", "LoadedWidth", c => c.Double());
            AlterColumn("dbo.Dispatching", "LoadedLength", c => c.Double());
            AlterColumn("dbo.Dispatching", "LoadedWeight", c => c.Double());
            AlterColumn("dbo.DispatchPoints", "DispatchingData_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.DispatchPoints", "DispatchingData_Id1");
            AddForeignKey("dbo.DispatchPoints", "DispatchingData_Id1", "dbo.Dispatching", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DispatchPoints", "DispatchingData_Id1", "dbo.Dispatching");
            DropIndex("dbo.DispatchPoints", new[] { "DispatchingData_Id1" });
            AlterColumn("dbo.DispatchPoints", "DispatchingData_Id", c => c.Int());
            AlterColumn("dbo.Dispatching", "LoadedWeight", c => c.Double(nullable: false));
            AlterColumn("dbo.Dispatching", "LoadedLength", c => c.Double(nullable: false));
            AlterColumn("dbo.Dispatching", "LoadedWidth", c => c.Double(nullable: false));
            AlterColumn("dbo.Dispatching", "LoadedHeight", c => c.Double(nullable: false));
            DropColumn("dbo.DispatchPoints", "DispatchingData_Id1");
            CreateIndex("dbo.DispatchPoints", "DispatchingData_Id");
            AddForeignKey("dbo.DispatchPoints", "DispatchingData_Id", "dbo.Dispatching", "Id");
        }
    }
}
