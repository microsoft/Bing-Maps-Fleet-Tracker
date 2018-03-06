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
    [Route("api/geofences")]
    [Authorize(UserRoles.Administrator)]
    public class GeoFencesController : ControllerBase
    {
        private readonly IGeoFenceService geoFenceService;
        private readonly IMapper dtoMapper;

        public GeoFencesController(
            IGeoFenceService geoFenceService,
            ILoggerFactory loggerFactory,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.geoFenceService = geoFenceService;
            this.dtoMapper = dtoMapper;
        }

        /// <summary>
        /// Query Geofences
        /// </summary>
        /// <param name="tags">Tags to search for</param>
        /// <param name="includesAllTags">True to return results including all tags, false to return results including any tags</param>
        /// <param name="name">Name of device</param>
        /// <returns>List of geofences</returns>
        // GET api/geofences
        [HttpGet]
        public async Task<IEnumerable<GeoFenceDto>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                var results = await this.geoFenceService.ListAsync();
                return this.dtoMapper.Map<IEnumerable<GeoFenceDto>>(results);
            }

            IEnumerable<GeoFence> taggedResults = null;
            if (!string.IsNullOrEmpty(tags))
            {
                var tagsArray = tags.Split(',');
                if (includesAllTags)
                {
                    taggedResults = await this.geoFenceService.FindContainingAllTagsAsync(tagsArray);
                }
                else
                {
                    taggedResults = await this.geoFenceService.FindContainingAnyTagsAsync(tagsArray);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return this.dtoMapper.Map<IEnumerable<GeoFenceDto>>(taggedResults);
            }

            var resultsByName = await this.geoFenceService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return this.dtoMapper.Map<IEnumerable<GeoFenceDto>>(resultsByName);
            }

            return this.dtoMapper.Map<IEnumerable<GeoFenceDto>>(
                taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id)));
        }

        /// <summary>
        /// Get geofence by Id
        /// </summary>
        /// <param name="id">The geofence Id</param>
        /// <returns>The geofence</returns>
        // GET api/geofences/5
        [HttpGet("{id}")]
        public async Task<GeoFenceDto> Get(int id)
        {
            var result = await this.geoFenceService.GetAsync(id);

            return this.dtoMapper.Map<GeoFenceDto>(result);
        }

        /// <summary>
        /// Create geofence
        /// </summary>
        /// <param name="geoFence">The geofence details</param>
        /// <returns>The created geofence</returns>
        // POST api/geofences
        [HttpPost]
        public async Task<GeoFenceDto> Post([FromBody]GeoFenceDto geoFence)
        {
            var model = this.dtoMapper.Map<GeoFence>(geoFence);

            var result = await this.geoFenceService.AddAsync(model);

            return this.dtoMapper.Map<GeoFenceDto>(result);
        }

        /// <summary>
        /// Create multiple geofences
        /// </summary>
        /// <param name="geoFences">List of geofence details</param>
        /// <returns>List of created geofences</returns>
        // POST api/geofences/batch
        [HttpPost("batch")]
        public async Task<IEnumerable<GeoFenceDto>> PostBatch([FromBody]GeoFenceDto[] geoFences)
        {
            var models = this.dtoMapper.Map<GeoFence[]>(geoFences);

            var results = await this.geoFenceService.AddAsync(models);

            return this.dtoMapper.Map<IEnumerable<GeoFenceDto>>(results);
        }

        /// <summary>
        /// Update existing geofence
        /// </summary>
        /// <param name="id">The geofence id</param>
        /// <param name="geoFence">The geofence details</param>
        /// <returns>The updated geofence</returns>
        // PUT api/geofences/1
        [HttpPut("{id}")]
        public async Task<GeoFenceDto> Put(int id, [FromBody]GeoFenceDto geoFence)
        {
            var model = this.dtoMapper.Map<GeoFence>(geoFence);

            var result = await this.geoFenceService.UpdateAsync(id, model);

            return this.dtoMapper.Map<GeoFenceDto>(result);
        }

        /// <summary>
        /// Delete geofence
        /// </summary>
        /// <param name="id">The geofence Id</param>
        /// <returns>Ok response</returns>
        // DELETE api/geofences/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.geoFenceService.DeleteAsync(id);
        }
    }
}
