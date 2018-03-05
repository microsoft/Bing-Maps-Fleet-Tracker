// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthorization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(),
                        Name = c.String(),
                        ClaimsId = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAtTimeUtc = c.DateTime(defaultValueSql: "GETUTCDATE()", nullable: false),
                        Role_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .Index(t => t.Role_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Role_Id", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "Role_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
        }
    }
}
