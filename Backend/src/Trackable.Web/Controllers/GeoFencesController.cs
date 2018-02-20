using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        public async Task<IEnumerable<GeoFence>> Get()
        {
            return await this.geoFenceService.ListAsync();
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

        // POST api/geofences
        [HttpPost("batch-geofences")]
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
