using System.Collections.Generic;

namespace Trackable.EntityFramework
{
    public interface ITaggedEntity
    {
        ICollection<TagData> Tags { get; set; }
    }
}
