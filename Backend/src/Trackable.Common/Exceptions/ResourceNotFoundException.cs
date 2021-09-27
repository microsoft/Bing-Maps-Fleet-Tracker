// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Common.Exceptions
{
    public class ResourceNotFoundException : ResourceExceptionBase
    {
        public ResourceNotFoundException(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
