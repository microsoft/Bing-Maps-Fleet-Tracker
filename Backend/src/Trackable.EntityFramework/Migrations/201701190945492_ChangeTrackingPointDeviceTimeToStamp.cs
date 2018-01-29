namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTrackingPointDeviceTimeToStamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackingPoints", "DeviceTimestampUtc", c => c.Long(nullable: false));
            DropColumn("dbo.TrackingPoints", "DeviceTimeUtc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackingPoints", "DeviceTimeUtc", c => c.DateTime(nullable: false));
            DropColumn("dbo.TrackingPoints", "DeviceTimestampUtc");
        }
    }
}
