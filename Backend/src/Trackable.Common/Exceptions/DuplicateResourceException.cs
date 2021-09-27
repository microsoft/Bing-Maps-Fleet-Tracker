// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Common.Exceptions
{
    public class DuplicateResourceException : ResourceExceptionBase
    {
        public DuplicateResourceException(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
