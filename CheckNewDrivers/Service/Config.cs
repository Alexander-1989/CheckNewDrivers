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

        public bool Read()
        {
            if (FileExists)
            {
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    Properties = serrializer.Deserialize(streamReader) as Properties;
                    return true;
                }
            }
            else
            {
                Properties = new Properties();
                return false;
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

        public bool FileExists => File.Exists(fileName);
    }
}