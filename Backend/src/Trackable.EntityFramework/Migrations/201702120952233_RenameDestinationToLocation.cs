namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDestinationToLocation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Destinations", newName: "Locations");
            RenameColumn(table: "dbo.Trips", name: "EndDestinationId", newName: "EndLocationId");
            RenameColumn(table: "dbo.Trips", name: "StartDestinationId", newName: "StartLocationId");
            RenameColumn(table: "dbo.TripLegs", name: "EndDestinationId", newName: "EndLocationId");
            RenameColumn(table: "dbo.TripLegs", name: "StartDestinationId", newName: "StartLocationId");
            RenameIndex(table: "dbo.Trips", name: "IX_StartDestinationId", newName: "IX_StartLocationId");
            RenameIndex(table: "dbo.Trips", name: "IX_EndDestinationId", newName: "IX_EndLocationId");
            RenameIndex(table: "dbo.TripLegs", name: "IX_StartDestinationId", newName: "IX_StartLocationId");
            RenameIndex(table: "dbo.TripLegs", name: "IX_EndDestinationId", newName: "IX_EndLocationId");
            DropPrimaryKey("dbo.Configurations");
            AddColumn("dbo.Configurations", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Configurations", "Namespace", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Configurations", new[] { "Id", "Namespace" });
            DropColumn("dbo.Configurations", "Key");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Configurations", "Key", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.Configurations");
            DropColumn("dbo.Configurations", "Namespace");
            DropColumn("dbo.Configurations", "Id");
            AddPrimaryKey("dbo.Configurations", "Key");
            RenameIndex(table: "dbo.TripLegs", name: "IX_EndLocationId", newName: "IX_EndDestinationId");
            RenameIndex(table: "dbo.TripLegs", name: "IX_StartLocationId", newName: "IX_StartDestinationId");
            RenameIndex(table: "dbo.Trips", name: "IX_EndLocationId", newName: "IX_EndDestinationId");
            RenameIndex(table: "dbo.Trips", name: "IX_StartLocationId", newName: "IX_StartDestinationId");
            RenameColumn(table: "dbo.TripLegs", name: "StartLocationId", newName: "StartDestinationId");
            RenameColumn(table: "dbo.TripLegs", name: "EndLocationId", newName: "EndDestinationId");
            RenameColumn(table: "dbo.Trips", name: "StartLocationId", newName: "StartDestinationId");
            RenameColumn(table: "dbo.Trips", name: "EndLocationId", newName: "EndDestinationId");
            RenameTable(name: "dbo.Locations", newName: "Destinations");
        }
    }
}
