namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveConnectionId : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Dispatching", newName: "Dispatches");
            RenameColumn(table: "dbo.DispatchPoints", name: "DispatchingData_Id1", newName: "DispatchData_Id");
            RenameIndex(table: "dbo.DispatchPoints", name: "IX_DispatchingData_Id1", newName: "IX_DispatchData_Id");
            AddColumn("dbo.DispatchPoints", "DispatchingDataId", c => c.Int(nullable: false));
            DropColumn("dbo.Dispatches", "ConnectionId");
            DropColumn("dbo.DispatchPoints", "DispatchingData_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DispatchPoints", "DispatchingData_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Dispatches", "ConnectionId", c => c.String(nullable: false));
            DropColumn("dbo.DispatchPoints", "DispatchingDataId");
            RenameIndex(table: "dbo.DispatchPoints", name: "IX_DispatchData_Id", newName: "IX_DispatchingData_Id1");
            RenameColumn(table: "dbo.DispatchPoints", name: "DispatchData_Id", newName: "DispatchingData_Id1");
            RenameTable(name: "dbo.Dispatches", newName: "Dispatching");
        }
    }
}
