// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Trackable.Models
{
    public class Configuration
    {
        public Configuration(string namespaceStr, string key, string description, object value)
        {
            this.Namespace = namespaceStr;
            this.Key = key;
            this.SerializedValue = JsonConvert.SerializeObject(value);
            this.Description = description;
        }

        internal Configuration(string key1, string key2, string description, string serializedValue)
        {
            this.Namespace = key1;
            this.Key = key2;
            this.SerializedValue = serializedValue;
            this.Description = description;
        }

        /// <summary>
        /// The configuration namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// The configuration key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The configuration description.
        /// </summary>
        [Mutable]
        public string Description { get; set; }

        /// <summary>
        /// The configuration's value.
        /// </summary>
        [Mutable]
        public string SerializedValue { get; private set; }

        public Type GetValue<Type>()
        {
            return JsonConvert.DeserializeObject<Type>(this.SerializedValue, GetSerializerSettings());
        }

        public object GetValue(Type t)
        {
            return JsonConvert.DeserializeObject(this.SerializedValue, t, GetSerializerSettings());
        }

        public void SetValue(object value)
        {
            this.SerializedValue = JsonConvert.SerializeObject(value, GetSerializerSettings());
        }

        private JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}
