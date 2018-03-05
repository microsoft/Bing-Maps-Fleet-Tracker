// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Common
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }
            
        public static string ThrowIfNullOrEmpty(this string obj, string paramName)
        {
            if (string.IsNullOrEmpty(obj))
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }
    }
}
