using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    // Used as a work around for multiple inheritance. Note that this is tied to DbRepositoryBase as it includes implementation.
    public interface IDbCountableRepository<TKey, TData, TModel> : IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>
        where TModel : ModelBase<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public static class IDbCountableRepositoryExtensions
    {
        public static async Task<int> GetCountAsync<TKey, TData, TModel>(this IDbCountableRepository<TKey, TData, TModel> repository)
            where TData : EntityBase<TKey>
            where TModel : ModelBase<TKey>
            where TKey : IEquatable<TKey>
        {
            return await repository.DbBaseRepository.FindBy(a => true).CountAsync();
        }
    }
}
