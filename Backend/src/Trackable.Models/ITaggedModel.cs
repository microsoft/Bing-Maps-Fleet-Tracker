using System.Collections.Generic;

namespace Trackable.Models
{
    public interface ITaggedModel
    {
        IEnumerable<string> Tags { get; set; }
    }
}
