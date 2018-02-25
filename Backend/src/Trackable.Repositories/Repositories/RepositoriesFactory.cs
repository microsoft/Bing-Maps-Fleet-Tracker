using AutoMapper;
using Trackable.EntityFramework;

namespace Trackable.Repositories
{
    public static class RepositoryFactory
    {
        public static Mapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ModelMappingProfile>()));
        public static IAssetRepository CreateAssetRepository(TrackableDbContext context)
        {
            return new AssetRepository(context, mapper);
        }

        public static IConfigurationRepository CreateConfigurationRepository(TrackableDbContext context)
        {
            return new ConfigurationRepository(context, mapper);
        }

        public static ILocationRepository CreateLocationRepository(TrackableDbContext context)
        {
            return new LocationRepository(context, mapper);
        }

        public static ITripRepository CreateTripRepository(TrackableDbContext context)
        {
            return new TripRepository(context, mapper);
        }

        public static ITrackingPointRepository CreateTrackingPointRepository(TrackableDbContext context)
        {
            return new TrackingPointRepository(context, mapper);
        }
    }
}
