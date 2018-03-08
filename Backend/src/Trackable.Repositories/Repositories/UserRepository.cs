// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{

    class UserRepository : DbRepositoryBase<string, UserData, User>, IUserRepository
    {

        public UserRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var data = await this.FindBy(u => u.Email == email)
                .FirstOrDefaultAsync();
            return this.ObjectMapper.Map<User>(data);
        }

        public async override Task<User> AddAsync(User model)
        {
            model.ThrowIfNull(nameof(model));

            var existingUser = await this.Db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                if (!existingUser.Deleted)
                {
                    throw new DuplicateResourceException("A user already exists with this email");
                }

                existingUser.Deleted = false;
                existingUser.Role = model.Role == null
                    ? await this.Db.Roles.SingleOrDefaultAsync(r => r.Name == UserRoles.Pending.ToString())
                    : await this.Db.Roles.SingleOrDefaultAsync(r => r.Id == model.Role.Id);

                await this.Db.SaveChangesAsync();
                return this.ObjectMapper.Map<User>(existingUser);
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var roleData = await this.Db.Roles.SingleOrDefaultAsync(r => r.Name == model.Role.Name);

            var dataModel = ObjectMapper.Map<UserData>(model);
            dataModel.Role = roleData;
            this.Db.Users.Add(dataModel);

            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<User>(dataModel);
        }

        public async override Task<User> UpdateAsync(string id, User model)
        {
            model.ThrowIfNull(nameof(model));

            var roleData = await this.Db.Roles.SingleOrDefaultAsync(r => r.Id == model.Role.Id);

            var modelData = await FindAsync(id);

            UpdateData(modelData, model);

            modelData.Role = roleData;

            await this.Db.SaveChangesAsync();

            return this.ObjectMapper.Map<User>(modelData);
        }

        public async Task<bool> AnyAsync()
        {
            return await this.Db.Users.AnyAsync();
        }

        protected override Expression<Func<UserData, object>>[] Includes => new Expression<Func<UserData, object>>[]
        {
            data => data.Role
        };
    }
}
