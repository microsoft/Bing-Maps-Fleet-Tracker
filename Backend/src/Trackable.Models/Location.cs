using System;
using System.Collections.Generic;
using Trackable.Models.Helpers;

namespace Trackable.Models
{
    [Serializable]
    public class Location : ModelBase<int>, IPoint, ITaggedModel, INamedModel
    {
        /// <summary>
        /// The Location's name.
        /// </summary>
        [Mutable]
        public string Name { get; set; }

        /// <summary>
        /// The address of the Location.
        /// </summary>
        [Mutable]
        public string Address { get; set; }

        /// <summary>
        /// Custom configured minimum wait time for the Location in seconds.
        /// </summary>
        [Mutable]
        public int? MinimumWaitTime { get; set; }

        /// <summary>
        /// Custom configured interest level for the Location.
        /// </summary>
        [Mutable]
        public InterestLevel InterestLevel { get; set; }

        [Mutable]
        public double Latitude { get; set; }

        [Mutable]
        public double Longitude { get; set; }

        [Mutable]
        public IEnumerable<string> Tags { get; set; }

        public object Clone()
        {
            return new Location()
            {
                Address = this.Address,
                InterestLevel = this.InterestLevel,
                MinimumWaitTime = this.MinimumWaitTime,
                Name = this.Name,
                Latitude = this.Latitude,
                Longitude = this.Longitude
            };
        }
    }

    /// <summary>
    /// InterestLevel enum defines the importance of a location. Used by Point
    /// of Interest based trip detectors to rank stops.
    /// 
    /// Unknown = No Interest level assigned
    /// AutoNew = Location was auto generated for a stop
    /// AutoLow = Location was auto generated but has seen moderate traffic
    /// AutoHigh = Location was auto generated but has seen high traffic
    /// Manual = Location was manually entered
    /// </summary>
    public enum InterestLevel
    {
        Unknown = 0,
        AutoNew = 10,
        AutoLow = 20,
        AutoHigh = 30,
        Manual = 40
    }
}
