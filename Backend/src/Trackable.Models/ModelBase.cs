// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Models
{
    [Serializable]
    public abstract class ModelBase<TKey>
    {
        public virtual TKey Id { get; set; }
    }
}
