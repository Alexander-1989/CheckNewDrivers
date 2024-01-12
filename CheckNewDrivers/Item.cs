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
            return CompareVersion(Version, other.Version);
        }

        public int CompareTo(string version)
        {
            return CompareVersion(Version, version);
        }

        public bool IsEmpty => Version == null || Version.Length == 0 || Href == null || Href.Length == 0;

        public override string ToString()
        {
            return Version;
        }

        protected virtual int CompareVersion(string version1, string version2)
        {
            int minLength = Math.Min(version1.Length, version2.Length);
            if (version1.Length > minLength)
            {
                version1 = version1.Substring(version1.Length - minLength, minLength);
            }
            else
            {
                version2 = version2.Substring(version2.Length - minLength, minLength);
            }
            return string.Compare(version1, version2);
        }
    }
}