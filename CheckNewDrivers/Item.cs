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
            return Version.CompareTo(other.Version);
        }

        public int CompareTo(string version)
        {
            return Version.CompareTo(version);
        }

        public bool IsNotEmpty
        {
            get
            {
                return Version != null &&
                    Version.Length > 0 &&
                    Href != null &&
                    Href.Length > 0;
            }
        }

        public override string ToString()
        {
            return Version;
        }
    }
}