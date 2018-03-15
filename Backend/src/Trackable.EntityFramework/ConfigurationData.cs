// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("Configurations")]
    public class ConfigurationData : EntityBase<string, string>
    {
        public string Description{ get; set; }

        public string Value { get; set; }
    }
}