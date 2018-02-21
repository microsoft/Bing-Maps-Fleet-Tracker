using System;

namespace Trackable.Models
{
    [Serializable]
    public abstract class ModelBase<TKey>
    {
        public virtual TKey Id { get; set; }
    }
}
