using System;

namespace CheckNewDrivers
{
    internal class Driver : IComparable<Driver>
    {
        public string Name { get; }
        public string Version { get; }
        public string Href { get; }

        public Driver() : this(string.Empty, string.Empty, string.Empty) { }

        public Driver(string name, string version, string href)
        {
            Name = name;
            Version = version;
            Href = href;
        }

        public int CompareTo(Driver other)
        {
            return CompareVersions(Version, other.Version);
        }

        public int CompareTo(string version)
        {
            return CompareVersions(Version, version);
        }

        public bool IsEmpty()
        {
            return Version == null || Version.Length == 0 || Href == null || Href.Length == 0;
        }

        public override string ToString()
        {
            return Version;
        }

        protected virtual int CompareVersions(string versionA, string versionB)
        {
            unsafe
            {
                if (versionA == null || versionB == null)
                {
                    throw new ArgumentNullException();
                }

                int lengthA = versionA.Length;
                int lengthB = versionB.Length;
                int minLength = lengthA < lengthB ? lengthA : lengthB;

                fixed (char* str1 = versionA, str2 = versionB)
                {
                    for (int i = lengthA - minLength, j = lengthB - minLength; i < lengthA; i++, j++)
                    {
                        char charA = *(str1 + i);
                        char charB = *(str2 + j);
                        if (charA < charB)
                        {
                            return -1;
                        }
                        else if (charA > charB)
                        {
                            return 1;
                        }
                    }
                }
            }
            return 0;
        }
    }
}