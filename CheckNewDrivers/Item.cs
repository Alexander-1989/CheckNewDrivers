using System;

namespace CheckNewDrivers
{
    internal class Item : IComparable<Item>
    {
        public string Version { get; }
        public string Href { get; }

        public Item() : this(string.Empty, string.Empty) { }

        public Item(string version, string href)
        {
            Version = version;
            Href = href;
        }

        public int CompareTo(Item other)
        {
            return CompareVersions(Version, other.Version);
        }

        public int CompareTo(string version)
        {
            return CompareVersions(Version, version);
        }

        public bool IsEmpty => Version == null || Version.Length == 0 || Href == null || Href.Length == 0;

        public override string ToString()
        {
            return Version;
        }

        protected virtual int CompareVersions(string versionA, string versionB)
        {
            if (versionA == null || versionB == null)
            {
                throw new ArgumentNullException();
            }

            int lengthA = versionA.Length;
            int lengthB = versionB.Length;
            int minLength = lengthA < lengthB ? lengthA : lengthB;

            for (int i = lengthA - minLength, j = lengthB - minLength; i < lengthA; i++, j++)
            {
                char charA = versionA[i];
                char charB = versionB[j];

                if (charA < charB)
                {
                    return -1;
                }
                else if (charA > charB)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}