using System;
using System.IO;
using System.Xml.Serialization;

namespace CheckNewDrivers.Service.Serializer
{
    internal class Configuration
    {
        public Properties Properties { get; set; }
        private const string defaultConfigName = "Config.xml";
        private readonly string fileName = null;
        private readonly XmlSerializer serrializer = new XmlSerializer(typeof(Properties));

        public Configuration() : this(defaultConfigName) { }

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
                    Properties = serrializer.Deserialize(streamReader) as Properties;
                }
            }
            else
            {
                Properties = new Properties();
            }
        }

        public void Write()
        {
            try
            {
                if (Properties != null)
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileName))
                    {
                        serrializer.Serialize(streamWriter, Properties);
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