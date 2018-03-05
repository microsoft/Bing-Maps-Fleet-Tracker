// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Configuration;

namespace Trackable.Func.Shared
{
    public static class Utils
    {
        public static string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings.Get(name);
        }
    }
}
