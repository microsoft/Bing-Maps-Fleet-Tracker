// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWebhooksToGeofences : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoFences", "Webhooks", c => c.String());
            AlterColumn("dbo.GeoFences", "Emails", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GeoFences", "Emails", c => c.String(nullable: false));
            DropColumn("dbo.GeoFences", "Webhooks");
        }
    }
}
