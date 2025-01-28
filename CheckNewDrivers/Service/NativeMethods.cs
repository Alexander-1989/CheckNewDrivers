using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CheckNewDrivers.Service
{
    internal static class NativeMethods
    {
        internal enum HWNDInsertAfter
        {
            HWND_BOTTOM = 1,
            HWND_NOTOPMOST = -2,
            HWND_TOP = 0,
            HWND_TOPMOST = -1
        }

        internal enum SWPFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        }

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, HWNDInsertAfter insertAfter, int x, int y, int width, int height, SWPFlags uFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT rectangle);
    }
}