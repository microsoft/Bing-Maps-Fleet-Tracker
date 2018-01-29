using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trackable.Common;
using Trackable.Models;
using Microsoft.Extensions.Logging;
using Trackable.Services;
using Microsoft.AspNetCore.Authorization;

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
