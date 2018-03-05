// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Trackable.Models
{
    public class Role : ModelBase<Guid>
    {
        public Role()
        {
            this.Id = Guid.NewGuid();
        }

        [JsonIgnore]
        public override Guid Id { get; set; }

        public string Name { get; set; }

    }
}
