using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trackable.Common;
using Trackable.Services;
using Trackable.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IEnumerable<Location>> Get()
        {
            return await this.locationService.ListAsync();
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

        //PUT api/locations/5
        [HttpPut("{id}")]
        public async Task<Location> Put(int id, [FromBody]Location location)
        {
            return await this.locationService.UpdateAsync(id, location);
        }
    }
}
