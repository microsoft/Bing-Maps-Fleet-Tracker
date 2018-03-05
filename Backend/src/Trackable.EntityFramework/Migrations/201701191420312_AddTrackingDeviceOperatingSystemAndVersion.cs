// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrackingDeviceOperatingSystemAndVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackingDevices", "OperatingSystem", c => c.String());
            AddColumn("dbo.TrackingDevices", "Version", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrackingDevices", "Version");
            DropColumn("dbo.TrackingDevices", "OperatingSystem");
        }
    }
}
