// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Trackable.Models
{
    public class JwtToken : ModelBase<Guid>
    {
        public JwtToken()
        {
            this.Id = Guid.NewGuid();
        }

        public IEnumerable<Claim> Claims { get; set; }

        public bool IsActive { get; set; }

        public string TrackingDeviceId { get; set; }

        public string UserId { get; set; }
    }
}
