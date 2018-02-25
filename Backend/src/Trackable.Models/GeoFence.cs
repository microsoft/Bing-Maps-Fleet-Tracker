using System.Collections.Generic;
using Trackable.Models.Helpers;

namespace Trackable.Models
{
    public class GeoFence : ModelBase<int>, ITaggedModel, INamedModel
    {
        /// <summary>
        /// A user friendly name for the GeoFence
        /// </summary>
        [Mutable]
        public string Name { get; set; }

        /// <summary>
        /// The email that will be notified when the geofence is breached.
        /// </summary>
        [Mutable]
        public IEnumerable<string> EmailsToNotify { get; set; }

        /// <summary>
        /// The cooldown in minutes between consecutive emails
        /// </summary>
        [Mutable]
        public long Cooldown { get; set; }

        /// <summary>
        /// The type of the geofence.
        /// </summary>
        [Mutable]
        public FenceType FenceType { get; set; }

        /// <summary>
        /// The area of the geofence.
        /// </summary>
        [Mutable]
        public IGeoFenceArea GeoFenceArea { get; set; }

        /// <summary>
        /// The ids of the assets constrained by this geo fence
        /// </summary>
        public IEnumerable<string> AssetIds { get; set; }

        [Mutable]
        public IEnumerable<string> Tags { get; set; }
    }


    /// <summary>
    /// Internal Fence triggers when a point is inside the fence
    /// External Fence triggers when a point is outside the fence
    /// </summary>
    public enum FenceType
    {
        Inbound = 0,
        Outbound = 1
    }
}
