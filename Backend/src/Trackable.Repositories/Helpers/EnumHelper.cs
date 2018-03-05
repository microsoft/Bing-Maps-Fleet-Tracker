// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Repositories.Helpers
{
    public static class EnumHelper
    {
        public static T ToEnum<T>(int enumValue) where T : IConvertible
        {
            if (Enum.IsDefined(typeof(T), enumValue))
                return (T)(object)enumValue;
            else
                return default(T);
        }

        public static T ToEnum<T>(int? enumValue) where T : IConvertible
        {
            if(!enumValue.HasValue || !Enum.IsDefined(typeof(T), enumValue.Value))
                return default(T);
            else
                return (T)(object)enumValue.Value;
        }
    }
}
