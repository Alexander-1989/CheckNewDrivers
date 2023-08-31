using System;
using System.IO;
using System.Xml.Serialization;

namespace CheckNewDrivers
{
    [Serializable]
    public class Properties
    {
        public string Address { get; set; } = "https://motu.com/en-us/download/product/408/";
    }

    internal class Configuration
    {
        public Properties fields = null;
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
            if (!File.Exists(fileName))
            {
                fields = new Properties();
            }
            else
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fields = serrializer.Deserialize(fs) as Properties;
                }
            }
        }

        public void Write()
        {
            try
            {
                if (fields != null)
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        serrializer.Serialize(fs, fields);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}