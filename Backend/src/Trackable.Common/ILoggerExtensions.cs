// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Trackable.Common.Exceptions;

namespace Trackable.Common
{
    public static class ILoggerExtensions
    {
        private static bool HeavyDebugEnabled { get; set; } = false;

        public static void SetHeavyDebugEnabled(this IApplicationBuilder factory, bool heavyDebugEnabled)
        {
            HeavyDebugEnabled = heavyDebugEnabled;
        }

        public static void LogDebugSerialize(this ILogger logger, string formatMessage, params object[] objects)
        {
            if (HeavyDebugEnabled)
            {
                logger.LogDebug(string.Format(formatMessage, SerializeObjects(objects)));
            }
            else
            {
                logger.LogDebug(formatMessage, objects);
            }
        }

        public static void LogError(this ILogger logger, Exception e)
        {
            logger.LogError("Exception occured with message {0} and exception {1}", e.Message, e.ToString());
        }

        public static void LogWarning(this ILogger logger, Exception e)
        {
            logger.LogWarning("Exception occured with message {0} and exception {1}", e.Message, e.ToString());
        }

        private static string[] SerializeObjects(object[] objects)
        {
            var serializedObjects = new string[objects.Length];
            var converter = JsonSerializer.Create(GetSerializerSettings());

            for (int i = 0; i < objects.Length; i++)
            {
                serializedObjects[i] = JsonConvert.SerializeObject(objects[i], GetSerializerSettings());
            }

            return serializedObjects;
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}
