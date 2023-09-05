using System;
using System.IO;
using System.Xml.Serialization;

namespace CheckNewDrivers
{
    [Serializable]
    public class Properties
    {
        public string Address { get; set; }
        private const string defAddress = "https://motu.com/en-us/download/product/408/";

        public Properties() : this(defAddress) { }

        public Properties(string address)
        {
            Address = address;
        }
    }

    internal class Configuration
    {
        public Properties properties = null;
        private const string defName = "Config.xml";
        private readonly string fileName = null;
        private readonly XmlSerializer serrializer = new XmlSerializer(typeof(Properties));

        public Configuration()
        {
            fileName = Path.Combine(Environment.CurrentDirectory, defName);
        }

        public Configuration(string FileName)
        {
            fileName = Path.GetFullPath(FileName);
        }

        public void Read()
        {
            if (File.Exists(fileName))
            {
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    properties = serrializer.Deserialize(streamReader) as Properties;
                }
            }
            else
            {
                properties = new Properties();
            }
        }

        public void Write()
        {
            try
            {
                if (properties != null)
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileName))
                    {
                        serrializer.Serialize(streamWriter, properties);
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