using System;
using System.IO;
using System.Xml.Serialization;

namespace CheckNewDrivers
{
    [Serializable]
    public class Properties
    {
        public string Address { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }

        private const string defaultForegroundColor = "Gray";
        private const string defaultBackgroundColor = "Black";
        private const string defaultAddress = "https://motu.com/en-us/download/product/408/";

        public Properties() : this(defaultAddress, defaultForegroundColor, defaultBackgroundColor) { }

        public Properties(string address) : this(address, defaultForegroundColor, defaultBackgroundColor) { }

        public Properties(string address, string foregroundColor) : this(address, foregroundColor, defaultBackgroundColor) { }

        public Properties(string address, string foregroundColor, string backgroundColor)
        {
            Address = address;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

    }

    internal class Configuration
    {
        public Properties Properties { get; set; }
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