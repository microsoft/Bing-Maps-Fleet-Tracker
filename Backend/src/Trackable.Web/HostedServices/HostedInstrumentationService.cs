// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Services
{
    class HostedInstrumentationService : HostedServiceBase
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public HostedInstrumentationService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var instrumentationService = scope.ServiceProvider.GetService<IInstrumentationService>();
                    await instrumentationService.PostInstrumentationAsync();
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }
    }
}
