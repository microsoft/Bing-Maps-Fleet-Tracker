// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
