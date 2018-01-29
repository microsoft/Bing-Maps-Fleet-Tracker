using Trackable.Common;
using System;
using System.Data.Entity.Migrations;
using System.Linq.Expressions;
using System.Linq;


namespace Trackable.EntityFramework.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<TrackableDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TrackableDbContext context)
        {
            foreach (var role in UserRoles.Roles)
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    context.Roles.Add(new RoleData()
                    {
                        Id = Guid.NewGuid(),
                        Name = role
                    });
                }
            }

            if(context.DeploymentId.Count() < 1)
            {
                context.DeploymentId.Add(new DeploymentIdData()
                {
                    Id = Guid.NewGuid()
                });
            }
        }
    }
}
