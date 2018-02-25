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
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService locationService;

        public LocationsController(ILoggerFactory loggerFactory, ILocationService locationService)
            : base(loggerFactory)
        {
            this.locationService = locationService.ThrowIfNull(nameof(locationService));
        }

        // GET api/locations
        [HttpGet]
        public async Task<IEnumerable<Location>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                return await this.locationService.ListAsync();
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
                return taggedResults;
            }

            var resultsByName = await this.locationService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return resultsByName;
            }

            return taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id));
        }

        // GET api/locations/5
        [HttpGet("{id}")]
        public async Task<Location> Get(int id)
        {
            return await this.locationService.GetAsync(id);
        }

        //GET api/locations/5/assetsCount
        [HttpGet("{id}/assetsCount")]
        public async Task<IDictionary<string, int>> GetAssetsCount(int id)
        {
            return await this.locationService.GetCountByAssetAsync(id);
        }

        //POST api/locations
        [HttpPost]
        public async Task<Location> Post([FromBody]Location location)
        {
            return await this.locationService.AddAsync(location);
        }

        //POST api/locations/batch
        [HttpPost("batch")]
        public async Task<IEnumerable<Location>> PostBatch([FromBody]Location[] locations)
        {
            return await this.locationService.AddAsync(locations);
        }

        //PUT api/locations/5
        [HttpPut("{id}")]
        public async Task<Location> Put(int id, [FromBody]Location location)
        {
            return await this.locationService.UpdateAsync(id, location);
        }
    }
}
