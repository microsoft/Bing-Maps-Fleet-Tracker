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
