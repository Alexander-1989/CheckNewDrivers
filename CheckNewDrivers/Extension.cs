using System;
using System.Collections.Generic;

namespace CheckNewDrivers
{
    static class Extension
    {
        public static T GetFirst<T>(this IEnumerable<T> items)
        {
            IEnumerator<T> enumerator = items?.GetEnumerator();
            return enumerator == null || !enumerator.MoveNext() ? default : enumerator.Current;
        }

        public static bool StartsWith(this string str, char value)
        {
            if (str == null || str.Length < 1)
            {
                throw new Exception($"{nameof(str)} is Null or Empty");
            }
            return value == str[0];
        }
    }
}