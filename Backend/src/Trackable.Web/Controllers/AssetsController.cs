// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.Web.Dtos;

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

        /// <summary>
        /// Query assets
        /// </summary>
        /// <param name="tags">Tags to search for</param>
        /// <param name="includesAllTags">True to return results including all tags, false to return results including any tags</param>
        /// <param name="name">Name of asset</param>
        /// <returns>List of assets</returns>
        // GET api/assets
        [HttpGet]
        public async Task<IEnumerable<AssetDto>> Get(
            [FromQuery] string tags = null,
            [FromQuery] bool includesAllTags = false,
            [FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(tags) && string.IsNullOrEmpty(name))
            {
                var results = await this.assetService.ListAsync();
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(results);
            }

            IEnumerable<Asset> taggedResults = null;
            if (!string.IsNullOrEmpty(tags))
            {
                var tagsArray = tags.Split(',');
                if (includesAllTags)
                {
                    taggedResults = await this.assetService.FindContainingAllTagsAsync(tagsArray);
                }
                else
                {
                    taggedResults = await this.assetService.FindContainingAnyTagsAsync(tagsArray);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(taggedResults);
            }

            var resultsByName = await this.assetService.FindByNameAsync(name);
            if (taggedResults == null)
            {
                return this.dtoMapper.Map<IEnumerable<AssetDto>>(resultsByName);
            }

            return this.dtoMapper.Map<IEnumerable<AssetDto>>(
                taggedResults.Where(d => resultsByName.Select(r => r.Id).Contains(d.Id)));
        }

        /// <summary>
        /// Get asset with the specified Id
        /// </summary>
        /// <param name="id">The id of the asset</param>
        /// <returns>The asset</returns>
        // GET api/assets/5
        [HttpGet("{id}")]
        public async Task<AssetDto> Get(string id)
        {
            var results = await this.assetService.GetAsync(id);

            return this.dtoMapper.Map<AssetDto>(results);
        }

        /// <summary>
        /// Get latest position of all assets
        /// </summary>
        /// <returns>Dictionary containing Asset Name vs last seen TrackingPoint</returns>
        // GET api/assets/all/positions
        [HttpGet("all/positions")]
        public async Task<IDictionary<string, TrackingPointDto>> GetLatestPositions()
        {
            var results = await this.assetService.GetAssetsLatestPositions();

            return this.dtoMapper.Map<IDictionary<string, TrackingPointDto>>(results);
        }

        /// <summary>
        /// Get the TrackingPoints belonging to the specified asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <returns>List of TrackingPoints</returns>
        // GET api/assets/5/points
        [HttpGet("{id}/points")]
        public async Task<IEnumerable<TrackingPointDto>> GetPoints(string id)
        {
            var results = await this.pointService.GetByAssetIdAsync(id);

            return this.dtoMapper.Map<IEnumerable<TrackingPointDto>>(results);
        }

        /// <summary>
        /// Get the trips done by the asset 
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <returns>List of Trips</returns>
        // GET api/assets/5/trips
        [HttpGet("{id}/trips")]
        public async Task<IEnumerable<TripDto>> GetTrips(string id)
        {
            var results = await this.tripService.GetByAssetIdAsync(id);

            return this.dtoMapper.Map<IEnumerable<TripDto>>(results);
        }

        /// <summary>
        /// Create a new asset
        /// </summary>
        /// <param name="asset">The asset details</param>
        /// <returns>The created asset</returns>
        // POST api/assets
        [HttpPost]
        public async Task<AssetDto> Post([FromBody]AssetDto asset)
        {
            var model = this.dtoMapper.Map<Asset>(asset);

            var result = await this.assetService.AddAsync(model);

            return this.dtoMapper.Map<AssetDto>(result);
        }

        /// <summary>
        /// Update an existing asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <param name="asset">The asset details</param>
        /// <returns>The asset</returns>
        // PUT api/assets/5
        [HttpPut("{id}")]
        public async Task<AssetDto> Put(string id, [FromBody]AssetDto asset)
        {
            var model = this.dtoMapper.Map<Asset>(asset);

            var result = await this.assetService.UpdateAsync(id, model);

            return this.dtoMapper.Map<AssetDto>(result);
        }

        /// <summary>
        /// Create multiple assets
        /// </summary>
        /// <param name="assets">List of asset details</param>
        /// <returns>List of assets</returns>
        // POST api/assets
        [HttpPost("batch")]
        public async Task<IEnumerable<AssetDto>> PostBatch([FromBody]AssetDto[] assets)
        {
            var models = this.dtoMapper.Map<Asset[]>(assets);

            var result = await this.assetService.AddAsync(await this.assetService.AddAsync(models));

            return this.dtoMapper.Map<IEnumerable<AssetDto>>(result);
        }

        /// <summary>
        /// Delete asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <returns>OK respoinse</returns>
        // DELETE api/assets/5
        [HttpDelete("{id}")]
        [Authorize(UserRoles.Administrator)]
        public async Task Delete(string id)
        {
            await this.assetService.DeleteAsync(id);
        }
    }
}
