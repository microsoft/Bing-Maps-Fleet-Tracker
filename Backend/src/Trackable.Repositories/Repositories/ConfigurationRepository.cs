using AutoMapper;
using System.Data.Entity;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{

    internal class ConfigurationRepository : DbRepositoryBase<string, string, ConfigurationData, Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(TrackableDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override async Task<Configuration> AddAsync(Configuration model)
        {
            var existingDeletedItem = await this.Db.Configurations.SingleOrDefaultAsync(d => d.Key1 == model.Namespace && d.Key2 == model.Key && d.Deleted);

            if (existingDeletedItem != null)
            {
                existingDeletedItem.Deleted = false;
                existingDeletedItem.Value = model.SerializedValue;
                existingDeletedItem.Description = model.Description;

                await this.Db.SaveChangesAsync();

                return this.ObjectMapper.Map<Configuration>(existingDeletedItem);
            }

            return await base.AddAsync(model);
        }
    }
}
