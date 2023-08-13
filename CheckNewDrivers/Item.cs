using System;

namespace CheckNewDrivers
{
    class Item : IComparable<Item>
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

        public override string ToString()
        {
            return Version;
        }
    }
}