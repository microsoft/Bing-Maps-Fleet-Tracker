using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Services
{
    public interface ICrudService<TKey, TModel>
    {
        Task<IEnumerable<TModel>> ListAsync();
        
        Task<TModel> GetAsync(TKey key);

        Task<TModel> AddAsync(TModel model);

        Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models);

        Task<TModel> UpdateAsync(TKey key, TModel model);

        Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models);

        Task DeleteAsync(TKey key);
    }
}
