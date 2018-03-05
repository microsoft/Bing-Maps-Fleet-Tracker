// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Linq.Expressions;
using Trackable.EntityFramework;
using Trackable.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Trackable.Repositories
{
    class TokenRepository : DbRepositoryBase<Guid, TokenData, JwtToken>, ITokenRepository
    {
        public TokenRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        protected override Expression<Func<TokenData, object>>[] Includes => null;

        public async Task DisableTokensByDeviceAsync(TrackingDevice device)
        {
            var activeTokens = await this.FindBy(t => t.TrackingDevice.Id == device.Id && t.IsActive)
                .ToListAsync();

            foreach(var token in activeTokens)
            {
                token.IsActive = false;
            }

            await this.Db.SaveChangesAsync();
        }

        public async Task DisableTokensByUserAsync(User user)
        {
            var activeTokens = await this.FindBy(t => t.User.Id == user.Id && t.IsActive)
                            .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.IsActive = false;
            }

            await this.Db.SaveChangesAsync();
        }

        public async Task<IEnumerable<JwtToken>> GetActiveByDeviceAsync(TrackingDevice device)
        {
            var tokens = await this.FindBy(t => t.TrackingDevice.Id == device.Id && t.IsActive)
                .ToListAsync();

            return tokens.Select(t => this.ObjectMapper.Map<JwtToken>(t));
        }

        public async Task<IEnumerable<JwtToken>> GetActiveByUserAsync(User user)
        {
            var tokens = await this.FindBy(t => t.User.Id == user.Id && t.IsActive)
                            .ToListAsync();

            return tokens.Select(t => this.ObjectMapper.Map<JwtToken>(t));
        }
    }
}
