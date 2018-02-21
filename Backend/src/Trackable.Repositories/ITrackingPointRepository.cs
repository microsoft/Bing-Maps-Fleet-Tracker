using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.EntityFramework;
using Trackable.Models;

namespace Trackable.Repositories
{
    public interface ITrackingPointRepository : IRepository<int, TrackingPoint>, IDbCountableRepository<int, TrackingPointData, TrackingPoint>
    {
        /// <summary>
        /// Gets all points by asset ID asynchronously.
        /// </summary>
        /// <param name="assetId">The asset ID.</param>
        /// <returns>The points async task.</returns>
        Task<IEnumerable<TrackingPoint>> GetByAssetIdAsync(string assetId);

        /// <summary>
        /// Gets all points by device ID asynchronously.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <returns>The points async task.</returns>
        Task<IEnumerable<TrackingPoint>> GetByDeviceIdAsync(string deviceId);

        /// <summary>
        /// Gets all points created after the specified UTC date-time.
        /// </summary>
        /// <param name="date">The specified UTC date-time.</param>
        /// <returns>The points async task.</returns>
        Task<IEnumerable<TrackingPoint>> GetByAssetIdAfterDateAsync(string assetId, DateTime date, bool includeDebug);

        /// <summary>
        /// Gets the latest tracking point that have not been assigned to a trip.
        /// </summary>
        /// <returns>The points async task.</returns>
        Task<TrackingPoint> GetByAssetIdLastLabeledAsync(string assetId);

        /// <summary>
        /// Gets the nearest tracking points to the specified trip, lat and long.
        /// </summary>
        /// <returns>The points async task.</returns>
        Task<IEnumerable<TrackingPoint>> GetNearestPoints(int tripId, IPoint point, int count);

        /// <summary>
        /// Gets the number of points per asset.
        /// </summary>
        /// <returns>The dictionary async task.</returns>
        Task<IDictionary<string, int>> PointsPerAssetCountAsync();

        /// <summary>
        /// Gets the number of points per day.
        /// </summary>
        /// <returns>The dictionary async task.</returns>
        Task<IDictionary<DateTime, int>> PointsPerDayCountAsync();

        /// <summary>
        /// Assign a list of tracking points to a particular trip
        /// </summary>
        /// <param name="tripId">The id of the trip</param>
        /// <param name="points">The list of tracking points</param>
        /// <returns>The async task.</returns>
        Task AssignPointsToTripAsync(int tripId, IEnumerable<TrackingPoint> points);
    }
}
