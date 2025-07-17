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

        public int X => Left;

        public int Y => Top;

        public int Width => Right - Left;

        public int Height => Bottom - Top;

        public Point Location => new Point(X, Y);

        public Size Size => new Size(Width, Height);

    }
}