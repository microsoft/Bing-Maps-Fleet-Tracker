namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDispatchingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dispatching",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(nullable: false),
                        ConnectionId = c.String(nullable: false),
                        AssetID = c.String(),
                        Avoid = c.String(),
                        Optimize = c.String(),
                        LoadedHeight = c.Double(nullable: false),
                        LoadedWidth = c.Double(nullable: false),
                        LoadedLength = c.Double(nullable: false),
                        LoadedWeight = c.Double(nullable: false),
                        AvoidCrossWind = c.Boolean(),
                        AvoidGroundingRisk = c.Boolean(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DispatchPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.Geography(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()" , nullable: false),
                        DispatchingData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dispatching", t => t.DispatchingData_Id)
                .Index(t => t.DispatchingData_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DispatchPoints", "DispatchingData_Id", "dbo.Dispatching");
            DropIndex("dbo.DispatchPoints", new[] { "DispatchingData_Id" });
            DropTable("dbo.DispatchPoints");
            DropTable("dbo.Dispatching");
        }
    }
}
