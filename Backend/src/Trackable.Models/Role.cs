// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Models
{
    public class Role : ModelBase<Guid>
    {
        public Role()
        {
            this.Id = Guid.NewGuid();
        }

        public string Name { get; set; }
    }
}
