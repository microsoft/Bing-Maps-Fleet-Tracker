// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace Trackable.EntityFramework
{
    // Required by command-line tools to be able to perform migrations.
    // https://docs.microsoft.com/en-us/aspnet/core/data/entity-framework-6#handle-connection-strings
    public class TrackableDbContextFactory : IDbContextFactory<TrackableDbContext>
    {
        public TrackableDbContext Create()
        {
            SqlProviderServices.SqlServerTypesAssemblyName =
               "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            return new TrackableDbContext("Server=(localdb)\\mssqllocaldb;Database=Trackable;Trusted_Connection=True;");
        }
    }
}
