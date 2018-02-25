using System;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    // Used as a work around for multiple inheritance. Note that this is tied to DbRepositoryBase as it includes implementation.
    public interface IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>
        where TModel : ModelBase<TKey>
        where TKey : IEquatable<TKey>
    {
        DbRepositoryBase<TKey, TData, TModel> DbBaseRepository { get; }
    }
}
