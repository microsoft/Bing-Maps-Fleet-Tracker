using System;
using System.Collections.Generic;

namespace Trackable.Common
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> elementsToAdd)
        {
            foreach (T item in elementsToAdd)
            {
                list.Add(item);
            }
        }
    }
}
