// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System.Linq;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories.AutoMapper
{
    class TokenUserResolver : IValueResolver<JwtToken, TokenData, UserData>
    {
        private readonly TrackableDbContext db;

        public TokenUserResolver(TrackableDbContext db)
        {
            this.db = db;
        }

        public UserData Resolve(JwtToken source, TokenData destination, UserData destMember, ResolutionContext context)
        {
            return this.db.Users.SingleOrDefault(a => !a.Deleted && a.Id == source.UserId);
        }
    }
}
