using System.Drawing;
using System.Runtime.InteropServices;

namespace CheckNewDrivers.Service
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Point Location => new Point(Left, Top);

        public Size Size => new Size(Right - Left, Bottom - Top);
    }
}
