using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    [Table("Tokens")]
    public class TokenData : EntityBase<Guid>
    {
        public string Value { get; set; }

        public bool IsActive { get; set; }

        public UserData User { get; set; }

        public TrackingDeviceData TrackingDevice { get; set; }
    }
}
