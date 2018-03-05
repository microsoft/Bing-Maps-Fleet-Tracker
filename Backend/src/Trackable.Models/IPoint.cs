// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;

namespace Trackable.Models
{
    public interface IPoint : ICloneable
    {
        /// <summary>
        /// The point location latitude.
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        /// The point location longitude.
        /// </summary>
        double Longitude { get; set; }
    }
}
