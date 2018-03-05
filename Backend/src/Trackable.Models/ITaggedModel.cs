// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Trackable.Models
{
    public interface ITaggedModel
    {
        IEnumerable<string> Tags { get; set; }
    }
}
