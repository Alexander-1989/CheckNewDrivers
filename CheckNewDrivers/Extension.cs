using System;
using System.Collections.Generic;

namespace CheckNewDrivers
{
    internal static class Extension
    {
        public static T GetFirst<T>(this IEnumerable<T> items, T initialValue = default)
        {
            if (items == null)
            {
                return initialValue;
            }

            if (items is IList<T> list && list.Count > 0)
            {
                return list[0];
            }

            using (IEnumerator<T> enumerator = items.GetEnumerator())
            {
                return enumerator.MoveNext() ? enumerator.Current : initialValue;
            }
        }

        public static T GetLast<T>(this IEnumerable<T> items, T initialValue = default)
        {
            if (items == null)
            {
                return initialValue;
            }

            if (items is IList<T> list && list.Count > 0)
            {
                return list[list.Count - 1];
            }

            T value;
            using (IEnumerator<T> enumerator = items.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    do
                    {
                        value = enumerator.Current;
                    }
                    while (enumerator.MoveNext());
                }
                else
                {
                    value = initialValue;
                }
            }

            return value;
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