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
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService assetService;
        private readonly ITrackingPointService pointService;
        private readonly ITripService tripService;

        public AssetsController(
            IAssetService assetService,
            ITrackingPointService pointService,
            ITripService tripService,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.assetService = assetService.ThrowIfNull(nameof(assetService));
            this.pointService = pointService.ThrowIfNull(nameof(pointService));
            this.tripService = tripService.ThrowIfNull(nameof(tripService));
        }

        // GET api/assets
        [HttpGet]
        public async Task<IEnumerable<Asset>> Get()
        {
            return await this.assetService.ListAsync();
        }

        // GET api/assets/5
        [HttpGet("{id}")]
        public async Task<Asset> Get(string id)
        {
            return await this.assetService.GetAsync(id);
        }

        // GET api/assets/all/positions
        [HttpGet("all/positions")]
        public async Task<IDictionary<string, TrackingPoint>> GetLatestPositions(string id)
        {
            return await this.assetService.GetAssetsLatestPositions();
        }

        // GET api/assets/5/points
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPoint>> GetPoints(string id)
        {
            return await this.pointService.GetByAssetIdAsync(id);
        }

        // GET api/assets/5/trips
        [HttpGet("{id}/trips")]
        public async Task<IEnumerable<Trip>> GetTrips(string id)
        {
            return await this.tripService.GetByAssetIdAsync(id);
        }

        // POST api/assets
        [HttpPost]
        public async Task<Asset> Post([FromBody]Asset asset)
        {
            return await this.assetService.AddAsync(asset);
        }

        // POST api/assets
        [HttpPost("batch")]
        public async Task<IEnumerable<Asset>> PostBatch([FromBody]Asset[] assets)
        {
            return await this.assetService.AddAsync(assets);
        }

        // DELETE api/assets/5
        [HttpDelete("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task Delete(string id)
        {
            await this.assetService.DeleteAsync(id);
        }
    }
}
