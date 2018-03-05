// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
