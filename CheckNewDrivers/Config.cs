using System;
using System.IO;
using System.Xml.Serialization;

namespace CheckNewDrivers
{
    [Serializable]
    public class Properties
    {
        public string Address { get; set; }

        public Properties() : this(string.Empty) { }

        public Properties(string address)
        {
            Address = address;
        }
    }

    internal class Configuration
    {
        public Properties properties = null;
        private readonly string fileName = null;
        private readonly XmlSerializer serrializer = new XmlSerializer(typeof(Properties));

        public Configuration()
        {
            fileName = Path.Combine(Environment.CurrentDirectory, "Config.xml");
        }

        public Configuration(string FileName)
        {
            fileName = Path.GetFullPath(FileName);
        }

        public void Read()
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    properties = serrializer.Deserialize(fs) as Properties;
                }
            }
            else
            {
                properties = new Properties("https://motu.com/en-us/download/product/408/");
            }
        }

        public void Write()
        {
            try
            {
                if (properties != null)
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        serrializer.Serialize(fs, properties);
                    }
                }
            }
            catch (Exception) { }
        }

        public bool FileExists()
        {
            return File.Exists(fileName);
        }
    }
}