// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Trackable.Repositories;

namespace Trackable.TripDetection
{
    public static class RepositoriesExtensions
    {
        public static IServiceCollection AddTripDetection(this IServiceCollection services, string bingMapsKey)
        {
            return services
                .AddTransient<ITripDetectorFactory, TripDetectorFactory>(container =>
                {
                    return new TripDetectorFactory(container.GetService<IConfigurationRepository>(),
                        container.GetService<ITripRepository>(),
                        container.GetService<ITrackingPointRepository>(),
                        container.GetService<ILocationRepository>(),
                        bingMapsKey);
                })
                .AddTransient<IPipeline, Pipeline>();
        }
    }
}
