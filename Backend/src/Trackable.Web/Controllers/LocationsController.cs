// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.Web.Dtos;

namespace Trackable.Web.Controllers
{
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService locationService;
        private readonly IMapper dtoMapper;

        public LocationsController(
            ILoggerFactory loggerFactory,
            ILocationService locationService,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.locationService = locationService;
            this.dtoMapper = dtoMapper;
        }

        // GET api/locations
        [HttpGet]
        public async Task<IEnumerable<LocationDto>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                var results = await this.locationService.ListAsync();
                return this.dtoMapper.Map<IEnumerable<LocationDto>>(results);
            }

            IEnumerable<Location> taggedResults = null;
            if (!string.IsNullOrEmpty(tags))
            {
                var tagsArray = tags.Split(',');
                if (includesAllTags)
                {
                    taggedResults = await this.locationService.FindContainingAllTagsAsync(tagsArray);
                }
                else
                {
                    taggedResults = await this.locationService.FindContainingAnyTagsAsync(tagsArray);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return this.dtoMapper.Map<IEnumerable<LocationDto>>(taggedResults);
            }

            var resultsByName = await this.locationService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return this.dtoMapper.Map<IEnumerable<LocationDto>>(resultsByName);
            }

            return this.dtoMapper.Map<IEnumerable<LocationDto>>(
                taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id)));
        }

        // GET api/locations/5
        [HttpGet("{id}")]
        public async Task<LocationDto> Get(int id)
        {
            var result = await this.locationService.GetAsync(id);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        //GET api/locations/5/assetsCount
        [HttpGet("{id}/assetsCount")]
        public async Task<IDictionary<string, int>> GetAssetsCount(int id)
        {
            return await this.locationService.GetCountByAssetAsync(id);
        }

        //POST api/locations
        [HttpPost]
        public async Task<LocationDto> Post([FromBody]LocationDto location)
        {
            var model = this.dtoMapper.Map<Location>(location);

            var result = await this.locationService.AddAsync(model);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        //POST api/locations/batch
        [HttpPost("batch")]
        public async Task<IEnumerable<LocationDto>> PostBatch([FromBody]LocationDto[] locations)
        {
            var models = this.dtoMapper.Map<Location[]>(locations);

            var results = await this.locationService.AddAsync(models);

            return this.dtoMapper.Map<IEnumerable<LocationDto>>(results);
        }

        //PUT api/locations/5
        [HttpPut("{id}")]
        public async Task<LocationDto> Put(int id, [FromBody]LocationDto location)
        {
            var model = this.dtoMapper.Map<Location>(location);

            var result = await this.locationService.UpdateAsync(id, model);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        // DELETE api/locations/5
        [HttpDelete("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task Delete(int id)
        {
            await this.locationService.DeleteAsync(id);
        }
    }
}
