// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Common.Exceptions
{
    public class BadArgumentException : ExceptionBase
    {
        public BadArgumentException(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
