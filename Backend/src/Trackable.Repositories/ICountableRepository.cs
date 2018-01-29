using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Repositories
{
    public interface ICountableRepository
    {
        Task<int> GetCountAsync();
    }
}
