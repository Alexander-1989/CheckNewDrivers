using System;

namespace CheckNewDrivers.Service
{
    internal class Driver : IComparable<Driver>
    {
        public string Name { get; }
        public string Version { get; }
        public string Href { get; }
        private const char EndChar = '\0';

        public Driver() : this(string.Empty, string.Empty, string.Empty) { }

        public Driver(string name, string version, string href)
        {
            Name = name;
            Version = version;
            Href = href;
        }

        public int CompareTo(Driver other)
        {
            return CompareTo(other.Version);
        }

        public int CompareTo(string version)
        {
            return CompareVersions(Version, version);
        }

        public bool Compare(Driver other)
        {
            return Compare(other.Version);
        }

        public bool Compare(string version)
        {
            return CompareVersions(Version, version) > 0;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(Href);
        }

        public override string ToString()
        {
            return Version;
        }

        protected virtual int CompareVersions(string versionA, string versionB)
        {
            if (versionA == null || versionB == null)
            {
                throw new ArgumentNullException("String is Null");
            }

            int minLength = Min(versionA.Length, versionB.Length);

            unsafe
            {
                fixed (char* stringA = versionA, stringB = versionB)
                {
                    char* ch1 = stringA + versionA.Length - minLength;
                    char* ch2 = stringB + versionB.Length - minLength;

                    while (*ch1 != EndChar && *ch2 != EndChar)
                    {
                        if (*ch1 < *ch2)
                        {
                            return -1;
                        }
                        else if (*ch1 > *ch2)
                        {
                            return 1;
                        }

                        ch1++;
                        ch2++;
                    }
                }
            }

            return 0;
        }

        private int Min(int x, int y)
        {
            return x < y ? x : y;
        }
    }
}