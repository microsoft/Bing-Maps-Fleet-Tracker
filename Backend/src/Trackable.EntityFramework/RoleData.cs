// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.EntityFramework
{
    [Table("Roles")]
    public class RoleData : EntityBase<Guid>
    {
        public string Name { get; set; }

        public ICollection<UserData> Users { get; set; }
    }
}
