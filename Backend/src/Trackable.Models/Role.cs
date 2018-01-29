using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
