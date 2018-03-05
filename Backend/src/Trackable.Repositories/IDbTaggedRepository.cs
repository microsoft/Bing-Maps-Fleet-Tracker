// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    // Used as a work around for multiple inheritance. Note that this is tied to DbRepositoryBase as it includes implementation.
    public interface IDbTaggedRepository<TKey, TData, TModel> : IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>, ITaggedEntity
        where TModel : ModelBase<TKey>, ITaggedModel
        where TKey : IEquatable<TKey>
    {
    }

    public static class IDbTaggedRepositoryExtensions
    {
        public static async Task<IEnumerable<TModel>> FindContainingAllTagsAsync<TKey, TData, TModel>(this IDbCountableRepository<TKey, TData, TModel> repository, IEnumerable<string> tags)
            where TData : EntityBase<TKey>, ITaggedEntity
            where TModel : ModelBase<TKey>, ITaggedModel
            where TKey : IEquatable<TKey>
        {
            if (tags == null || !tags.Any())
            {
                throw new BadArgumentException("Tags cannot be empty");
            }

            var data = await repository.DbBaseRepository.FindBy(a => tags.All(t => a.Tags.Select(s => s.TagName).Contains(t))).ToListAsync();

            return repository.DbBaseRepository.ObjectMapper.Map<IEnumerable<TModel>>(data);
        }

        public static async Task<IEnumerable<TModel>> FindContainingAnyTagsAsync<TKey, TData, TModel>(this IDbCountableRepository<TKey, TData, TModel> repository, IEnumerable<string> tags)
            where TData : EntityBase<TKey>, ITaggedEntity
            where TModel : ModelBase<TKey>, ITaggedModel
            where TKey : IEquatable<TKey>
        {
            if (tags == null || !tags.Any())
            {
                throw new BadArgumentException("Tags cannot be empty");
            }

            var data = await repository.DbBaseRepository.FindBy(a => tags.Any(t => a.Tags.Select(s => s.TagName).Contains(t))).ToListAsync();

            return repository.DbBaseRepository.ObjectMapper.Map<IEnumerable<TModel>>(data);
        }
    }
}
