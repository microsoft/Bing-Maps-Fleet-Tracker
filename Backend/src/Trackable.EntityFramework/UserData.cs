using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.EntityFramework
{
    [Table("Users")]
    public class UserData : EntityBase<Guid>
    {
        [Required]
        [MaxLength(450)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        public string Name { get; set; }

        public string ClaimsId { get; set; }

        public RoleData Role { get; set; }
    }
}
