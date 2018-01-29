using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Trackable.Repositories;

namespace Trackable.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string dbConnectionString, string rootPath)
        {
            return services
                .AddRepositories(dbConnectionString, rootPath)
                .AddTransient<IAssetService, AssetService>()
                .AddTransient<ILocationService, LocationService>()
                .AddTransient<ITrackingDeviceService, TrackingDeviceService>()
                .AddTransient<ITrackingPointService, TrackingPointService>()
                .AddTransient<ITripService, TripService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IReportingService, ReportingService>()
                .AddTransient<IGeoFenceService, GeoFenceService>()
                .AddTransient<INotificationService, NotificationService>()
                .AddTransient<IDispatchingService, DispatchingService>()
                .AddTransient<IInstrumentationService, InstrumentationService>()
                .AddTransient<ITokenService, JwtTokenService>()
                .AddSingleton<ISettingsService, SettingsService>();
        }

        public static IApplicationBuilder UseServices(this IApplicationBuilder builder, string connectionString, string ownerEmail)
        {
            UserService.OwnerEmail = ownerEmail;
            return builder.UseRepositories(connectionString);
        }
    }
}
