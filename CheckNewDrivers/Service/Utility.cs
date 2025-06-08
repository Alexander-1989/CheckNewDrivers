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
    }
}