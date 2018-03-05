// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompositeKeyEntityBase : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Configurations");
            AddColumn("dbo.Configurations", "Key1", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Configurations", "Key2", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Configurations", new[] { "Key1", "Key2" });
            DropColumn("dbo.Configurations", "Id");
            DropColumn("dbo.Configurations", "Namespace");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Configurations", "Namespace", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Configurations", "Id", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.Configurations");
            DropColumn("dbo.Configurations", "Key2");
            DropColumn("dbo.Configurations", "Key1");
            AddPrimaryKey("dbo.Configurations", new[] { "Id", "Namespace" });
        }
    }
}
