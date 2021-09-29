// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace Trackable.Web.Dtos
{
    public class LocationDto
    {
        /// <summary>
        /// Location id, Autogenerated or Specified
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Location Name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Location Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Location Latitude
        /// </summary>
        [Required]
        public double Latitude { get; set; }
        
        /// <summary>
        /// Location Longitude
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
