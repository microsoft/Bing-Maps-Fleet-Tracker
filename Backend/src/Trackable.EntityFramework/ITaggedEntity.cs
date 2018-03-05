// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Trackable.EntityFramework
{
    public interface ITaggedEntity
    {
        ICollection<TagData> Tags { get; set; }
    }
}
