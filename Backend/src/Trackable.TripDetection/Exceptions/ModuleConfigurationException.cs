// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.TripDetection.Exceptions
{
    public class ModuleConfigurationException : Exception
    {
        public ModuleConfigurationException(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
