// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Common.Exceptions
{
    public class ExceptionBase : Exception
    {
        public ExceptionBase(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {

        }
    }
}
