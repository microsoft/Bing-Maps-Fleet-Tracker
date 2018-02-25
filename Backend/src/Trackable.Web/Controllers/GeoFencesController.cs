using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;

namespace Trackable.Web.Controllers
{
    [Route("api/geofences")]
    [Authorize(UserRoles.Administrator)]
    public class GeoFencesController : ControllerBase
    {
        private readonly IGeoFenceService geoFenceService;

        public GeoFencesController(
            IGeoFenceService geoFenceService,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.geoFenceService = geoFenceService.ThrowIfNull(nameof(geoFenceService));
        }

        // GET api/geofences
        [HttpGet]
        public async Task<IEnumerable<GeoFence>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                return await this.geoFenceService.ListAsync();
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
                return taggedResults;
            }

            var resultsByName = await this.geoFenceService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return resultsByName;
            }

            return taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id));
        }

        // GET api/geofences/5
        [HttpGet("{id}")]
        public async Task<GeoFence> Get(int id)
        {
            return await this.geoFenceService.GetAsync(id);
        }

        // POST api/geofences
        [HttpPost]
        public async Task<GeoFence> Post([FromBody]GeoFence geoFence)
        {
            return await this.geoFenceService.AddAsync(geoFence);
        }

        // POST api/geofences/batch
        [HttpPost("batch")]
        public async Task<IEnumerable<GeoFence>> PostBatch([FromBody]GeoFence[] geoFences)
        {
            return await this.geoFenceService.AddAsync(geoFences);
        }

        [HttpPut("{id}")]
        public async Task<GeoFence> Put(int id, [FromBody]GeoFence geoFence)
        {
            return await this.geoFenceService.UpdateAsync(id, geoFence);
        }

        // DELETE api/geofences/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.geoFenceService.DeleteAsync(id);
        }
    }
}
