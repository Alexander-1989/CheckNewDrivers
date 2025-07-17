using System;

namespace CheckNewDrivers.Service
{
    [Serializable]
    public class Properties
    {
        public Rectangle Rectangle { get; set; }
        public string Address { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }

        private const string defaultForegroundColor = "Gray";
        private const string defaultBackgroundColor = "Black";
        private const string defaultAddress = "https://motu.com/en-us/download/product/408/";
        private const int defaultX = 0;
        private const int defaultY = 0;
        private const int defaultWidth = 1000;
        private const int defaultHeight = 500;

        public Properties() : this(defaultX, defaultY, defaultWidth, defaultHeight, defaultAddress, defaultForegroundColor, defaultBackgroundColor) { }

        public Properties(int x, int y, int width, int height, string address, string foregroundColor, string backgroundColor)
        {
            Rectangle = new Rectangle()
            {
                Left = x,
                Top = y,
                Right = x + width,
                Bottom = y + height
            };
            Address = address;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}