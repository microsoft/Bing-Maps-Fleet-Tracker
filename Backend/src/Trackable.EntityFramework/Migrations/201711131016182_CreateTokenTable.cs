// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTokenTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Value = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                        TrackingDevice_Id = c.String(maxLength: 128),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrackingDevices", t => t.TrackingDevice_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.TrackingDevice_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Tokens", "TrackingDevice_Id", "dbo.TrackingDevices");
            DropIndex("dbo.Tokens", new[] { "User_Id" });
            DropIndex("dbo.Tokens", new[] { "TrackingDevice_Id" });
            DropTable("dbo.Tokens");
        }
    }
}
