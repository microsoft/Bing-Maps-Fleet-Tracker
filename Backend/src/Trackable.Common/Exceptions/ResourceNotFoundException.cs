// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common.Exceptions;

namespace Trackable.Common.Exceptions
{
    public class ResourceNotFoundException : ResourceExceptionBase
    {
        public ResourceNotFoundException(string userFriendlyMessage)
            :base(userFriendlyMessage)
        {
        }
    }
}
