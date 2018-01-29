using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Trackable.EntityFramework
{
    [Table("Configurations")]
    public class ConfigurationData : EntityBase<string, string>
    {
        public string Description{ get; set; }

        public string Value { get; set; }
    }
}