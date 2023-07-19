using System.Collections.Generic;

namespace CheckNewDrivers
{
    static class Extension
    {
        public static T First<T>(this IEnumerable<T> items)
        {
            IEnumerator<T> enumerator = items?.GetEnumerator();
            return enumerator == null || !enumerator.MoveNext() ? default : enumerator.Current;
        }
    }
}