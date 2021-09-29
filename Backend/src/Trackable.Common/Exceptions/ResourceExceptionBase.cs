// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Common.Exceptions
{
    public class ResourceExceptionBase : ExceptionBase
    {
        public ResourceExceptionBase(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
