using System;
using System.Runtime.InteropServices;

namespace CheckNewDrivers.Service
{
    internal static class NativeMethods
    {
        internal enum SWPInsertAfter
        {
            NOTOPMOST   = -2,
            TOPMOST     = -1,
            TOP         =  0,
            BOTTOM      =  1,
        }

        internal enum SWPFlags : uint
        {
            ASYNCWINDOWPOS 	= 0x4000,
            DEFERERASE      = 0x2000,
            DRAWFRAME       = 0x0020,
            FRAMECHANGED    = 0x0020,
            HIDEWINDOW      = 0x0080,
            NOACTIVATE      = 0x0010,
            NOCOPYBITS      = 0x0100,
            NOMOVE          = 0x0002,
            NOOWNERZORDER   = 0x0200,
            NOREDRAW        = 0x0008,
            NOREPOSITION    = 0x0200,
            NOSENDCHANGING  = 0x0400,
            NOSIZE          = 0x0001,
            NOZORDER        = 0x0004,
            SHOWWINDOW      = 0x0040
        }

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, SWPInsertAfter insertAfter, int x, int y, int width, int height, SWPFlags uFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rectangle);
    }
}