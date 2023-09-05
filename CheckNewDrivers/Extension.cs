using System;
using System.Collections.Generic;

namespace CheckNewDrivers
{
    internal static class Extension
    {
        public static T GetFirst<T>(this IEnumerable<T> items, T initialValue = default)
        {
            IEnumerator<T> enumerator = items?.GetEnumerator();
            return enumerator == null || !enumerator.MoveNext() ? initialValue : enumerator.Current;
        }

        public static bool StartsWith(this string str, char value)
        {
            if (str == null || str.Length < 1)
            {
                throw new Exception($"String {nameof(str)} is Null or Empty");
            }
            return str[0] == value;
        }
    }
}