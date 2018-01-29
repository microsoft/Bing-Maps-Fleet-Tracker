using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Trackable.EntityFramework;
using AutoMapper;
using Trackable.Repositories.AutoMapper;

namespace Trackable.Repositories
{
    public static class RepositoriesExtensions
    {
        public static IServiceCollection AddRepositories(
            this IServiceCollection services,
            string dbConnectionString,
            string rootPath)
        {
            return services
                .AddDbContext(dbConnectionString, rootPath)

                .AddTransient<IAssetRepository, AssetRepository>()
                .AddTransient<ITrackingDeviceRepository, TrackingDeviceRepository>()
                .AddTransient<ITrackingPointRepository, TrackingPointRepository>()
                .AddTransient<IConfigurationRepository, ConfigurationRepository>()
                .AddTransient<ILocationRepository, LocationRepository>()
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IRoleRepository, RoleRepository>()
                .AddTransient<ITripRepository, TripRepository>()
                .AddTransient<IGeoFenceRepository, GeoFenceRepository>()
                .AddTransient<IGeoFenceUpdateRepository, GeoFenceUpdateRepository>()
                .AddTransient<IAssetPropertiesRepository, AssetPropertiesRepository>()
                .AddTransient<IInstrumentationRepository, InstrumentationRepository>()
                .AddTransient<ITokenRepository, TokenRepository>()

                .AddSingleton<MapperConfiguration>(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()))
                
                .AddScoped<TrackingDeviceAssetResolver>()
                .AddScoped<TokenTrackingDeviceResolver>()
                .AddScoped<TokenUserResolver>()
                
                .AddScoped<IMapper>(ctx => new Mapper(ctx.GetService<MapperConfiguration>(), t => ctx.GetService(t)));
        }

        public static IApplicationBuilder UseRepositories(this IApplicationBuilder builder, string connectionString)
        {
            return builder.UseDb(connectionString);
        }
    }
}
