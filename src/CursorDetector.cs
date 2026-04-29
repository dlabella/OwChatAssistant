using System.Runtime.InteropServices;

public static class CursorDetector
    {
        private const int CURSOR_SHOWING = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(
            out CURSORINFO pci);

        public static bool IsCursorVisible()
        {
            CURSORINFO info = new();
            info.cbSize = Marshal.SizeOf(info);
            var shown = false;
            if (GetCursorInfo(out info))
            {
                shown = (info.flags & CURSOR_SHOWING) != 1;
            }
            Console.WriteLine($"Cursor is shown: {shown}");
            return shown;
        }
    }