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

        /// <summary>
        /// Query locations
        /// </summary>
        /// <param name="tags">Tags to search for</param>
        /// <param name="includesAllTags">True to return results including all tags, false to return results including any tags</param>
        /// <param name="name">Name of device</param>
        /// <returns>List of locations</returns>
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

        /// <summary>
        /// Get location by id
        /// </summary>
        /// <param name="id">The location id</param>
        /// <returns>The location</returns>
        // GET api/locations/5
        [HttpGet("{id}")]
        public async Task<LocationDto> Get(string id)
        {
            var result = await this.locationService.GetAsync(id);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        /// <summary>
        /// Get the number of times assets have visited a location
        /// </summary>
        /// <param name="id">The location id</param>
        /// <returns>Dictionary containing asset names vs visit count</returns>
        //GET api/locations/5/assetsCount
        [HttpGet("{id}/assetsCount")]
        public async Task<IDictionary<string, int>> GetAssetsCount(string id)
        {
            return await this.locationService.GetCountByAssetAsync(id);
        }

        /// <summary>
        /// Create location
        /// </summary>
        /// <param name="location">The location details</param>
        /// <returns>The created location</returns>
        //POST api/locations
        [HttpPost]
        public async Task<LocationDto> Post([FromBody]LocationDto location)
        {
            var model = this.dtoMapper.Map<Location>(location);

            var result = await this.locationService.AddAsync(model);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        /// <summary>
        /// Create multiple locations
        /// </summary>
        /// <param name="locations">List of location details</param>
        /// <returns>The created locations</returns>
        //POST api/locations/batch
        [HttpPost("batch")]
        public async Task<IEnumerable<LocationDto>> PostBatch([FromBody]LocationDto[] locations)
        {
            var models = this.dtoMapper.Map<Location[]>(locations);

            var results = await this.locationService.AddAsync(models);

            return this.dtoMapper.Map<IEnumerable<LocationDto>>(results);
        }

        /// <summary>
        /// Update existing location 
        /// </summary>
        /// <param name="id">The location id</param>
        /// <param name="location">The location details</param>
        /// <returns>The updated location</returns>
        //PUT api/locations/5
        [HttpPut("{id}")]
        public async Task<LocationDto> Put(string id, [FromBody]LocationDto location)
        {
            var model = this.dtoMapper.Map<Location>(location);

            var result = await this.locationService.UpdateAsync(id, model);

            return this.dtoMapper.Map<LocationDto>(result);
        }

        /// <summary>
        /// Delete location
        /// </summary>
        /// <param name="id">The location id</param>
        /// <returns>Ok response</returns>
        // DELETE api/locations/5
        [HttpDelete("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task Delete(string id)
        {
            await this.locationService.DeleteAsync(id);
        }
    }
}
