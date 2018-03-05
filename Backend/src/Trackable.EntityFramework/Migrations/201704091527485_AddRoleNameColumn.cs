// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleNameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "Name");
        }
    }
}
