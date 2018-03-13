// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace Trackable.Web.Dtos
{
    public class RoleDto
    {
        /// <summary>
        /// Role Name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
