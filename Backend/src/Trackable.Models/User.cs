// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Trackable.Models
{
    public class User : ModelBase<Guid>
    {
        public User()
        {
            this.Id = Guid.NewGuid();
        }

        public string Email { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string ClaimsId { get; set; }

        public Role Role { get; set; }
    }
}
