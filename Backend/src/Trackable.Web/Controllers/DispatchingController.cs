// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Trackable.Services;
using Trackable.Web.Dtos;
using Trackable.Models;

namespace Trackable.Web.Controllers
{
    [Route("api/dispatching")]
    public class DispatchingController : ControllerBase
    {
        private readonly IDispatchingService dispatchingService;
        private readonly IAssetService assetService;
        private readonly IMapper dtoMapper;

        public DispatchingController(
            ILoggerFactory loggerFactory,
            IDispatchingService dispatchingService,
            IAssetService assetService,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.dispatchingService = dispatchingService;
            this.assetService = assetService;
            this.dtoMapper = dtoMapper;
        }

        /// <summary>
        /// Dispatch according to route
        /// </summary>
        /// <param name="dispatchingParameters">The parameters required for dispatching</param>
        /// <returns>Route results</returns>
        // Post api/dispatching
        [HttpPost]
        public async Task<IEnumerable<DispatchingResults>> Post([FromBody]DispatchDto dispatchingParameters)
        {
            var dispatchModel = this.dtoMapper.Map<Dispatch>(dispatchingParameters);
            var asset = await this.assetService.GetAsync(dispatchingParameters.AssetId);
            return await this.dispatchingService.CallRoutingAPI(dispatchModel, asset.AssetProperties);
        }
    }
}
