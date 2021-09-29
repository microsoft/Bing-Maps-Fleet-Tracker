// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Configurations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConfigurableAttribute : Attribute
    {
        private readonly object defaultValue;
        private readonly string description;

        public ConfigurableAttribute(object defaultValue, string description = "")
        {
            this.defaultValue = defaultValue;
            this.description = description;
        }
    }
}
