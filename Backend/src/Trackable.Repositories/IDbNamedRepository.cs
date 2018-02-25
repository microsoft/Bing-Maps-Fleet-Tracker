using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    // Used as a work around for multiple inheritance. Note that this is tied to DbRepositoryBase as it includes implementation.
    public interface IDbNamedRepository<TKey, TData, TModel> : IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>, INamedEntity
        where TModel : ModelBase<TKey>, INamedModel
        where TKey : IEquatable<TKey>
    {
    }

    public static class IDbNamedRepositoryExtensions
    {
        public static async Task<IEnumerable<TModel>> FindByNameAsync<TKey, TData, TModel>(this IDbCountableRepository<TKey, TData, TModel> repository, string name)
            where TData : EntityBase<TKey>, INamedEntity
            where TModel : ModelBase<TKey>, INamedModel
            where TKey : IEquatable<TKey>
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new BadArgumentException("Name cannot be empty");
            }

            var data = await repository.DbBaseRepository.FindBy(a => a.Name == name).ToListAsync();

            return repository.DbBaseRepository.ObjectMapper.Map<IEnumerable<TModel>>(data);
        }
    }
}
