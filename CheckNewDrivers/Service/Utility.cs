using System;
using System.Net;

namespace CheckNewDrivers.Service.Utilities
{
    internal static class Utility
    {
        public static int Max(int x, int y)
        {
            return x > y ? x : y;
        }

        public static int Min(int x, int y)
        {
            return x < y ? x : y;
        }

        public static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        public static int OrderByAscending<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b);
        }

        public static int OrderByDescending<T>(T a, T b) where T : IComparable<T>
        {
            return b.CompareTo(a);
        }

        public static void SetSecurityProtocol()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Ssl3;
        }
    }
}