using System.Runtime.InteropServices;

namespace System.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Point Location => new Point(Left, Top);

        public Size Size => new Size(Right - Left, Bottom - Top);
    }
}
