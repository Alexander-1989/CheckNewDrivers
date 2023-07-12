namespace CheckNewDrivers
{
    class Item
    {
        public string Version { get; }
        public string Href { get; }

        public Item() : this(string.Empty, string.Empty) { }

        public Item(string version, string href)
        {
            Version = version;
            Href = href;
        }

        public override string ToString()
        {
            return Version;
        }
    }
}