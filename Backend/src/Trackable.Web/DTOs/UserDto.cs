using System;
using Trackable.Models;

namespace Trackable.Web.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public Role Role { get; set; }
    }
}
