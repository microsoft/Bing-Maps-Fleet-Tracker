using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.Web.DTOs;

namespace Trackable.Web.Controllers
{
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService assetService;
        private readonly ITrackingPointService pointService;
        private readonly ITripService tripService;
        private readonly IMapper dtoMapper;

        public AssetsController(
            IAssetService assetService,
            ITrackingPointService pointService,
            ITripService tripService,
            ILoggerFactory loggerFactory,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.assetService = assetService;
            this.pointService = pointService;
            this.tripService = tripService;
            this.dtoMapper = dtoMapper;
        }

        // GET api/assets
        [HttpGet]
        public async Task<IEnumerable<AssetDto>> Get([FromQuery] string tags = null, [FromQuery] bool includesAllTags = false)
        {
            if (string.IsNullOrEmpty(tags))
            {
                var results = await this.assetService.ListAsync();
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(results);
            }

            var tagsArray = tags.Split(',');
            if (includesAllTags)
            {
                var results = await this.assetService.FindContainingAllTagsAsync(tagsArray);
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(results);
            }
            else
            {
                var results = await this.assetService.FindContainingAnyTagsAsync(tagsArray);
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(results);
            }
        }

        // GET api/assets/5
        [HttpGet("{id}")]
        public async Task<AssetDto> Get(string id)
        {
            var results = await this.assetService.GetAsync(id);

            return this.dtoMapper.Map<AssetDto>(results);
        }

        // GET api/assets/all/positions
        [HttpGet("all/positions")]
        public async Task<IDictionary<string, TrackingPointDto>> GetLatestPositions()
        {
            var results = await this.assetService.GetAssetsLatestPositions();

            return this.dtoMapper.Map<IDictionary<string, TrackingPointDto>>(results);
        }

        // GET api/assets/5/points
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPointDto>> GetPoints(string id)
        {
            var results = await this.pointService.GetByAssetIdAsync(id);

            return this.dtoMapper.Map<IEnumerable<TrackingPointDto>>(results);
        }

        // GET api/assets/5/trips
        [HttpGet("{id}/trips")]
        public async Task<IEnumerable<TripDto>> GetTrips(string id)
        {
            var results = await this.tripService.GetByAssetIdAsync(id);

            return this.dtoMapper.Map<IEnumerable<TripDto>>(results);
        }

        // POST api/assets
        [HttpPost]
        public async Task<AssetDto> Post([FromBody]AssetDto asset)
        {
            var model = this.dtoMapper.Map<Asset>(asset);

            var result = await this.assetService.AddAsync(model);

            return this.dtoMapper.Map<AssetDto>(result);
        }

        // PUT api/assets/5
        [HttpPut("{id}")]
        public async Task<AssetDto> Put(string id, [FromBody]AssetDto asset)
        {
            var model = this.dtoMapper.Map<Asset>(asset);

            var result = await this.assetService.UpdateAsync(id, model);

            return this.dtoMapper.Map<AssetDto>(result);
        }

        // POST api/assets
        [HttpPost("batch")]
        public async Task<IEnumerable<AssetDto>> PostBatch([FromBody]AssetDto[] assets)
        {
            var models = this.dtoMapper.Map<Asset[]>(assets);

            var result = await this.assetService.AddAsync(await this.assetService.AddAsync(models));

            return this.dtoMapper.Map<IEnumerable<AssetDto>>(result);
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
