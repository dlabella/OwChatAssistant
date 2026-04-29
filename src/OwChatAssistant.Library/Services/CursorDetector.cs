using System.Runtime.InteropServices;

namespace OwChatAssistant.Library.Services;

public static class CursorDetector
{
    private const int CURSOR_SHOWING = 0x00000001;

    [StructLayout(LayoutKind.Sequential)]
    private struct CursorInfo
    {
        public int cbSize;
        public int flags;
        public IntPtr hCursor;
        public Point ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorInfo(
        out CursorInfo pci);

    public static bool IsCursorVisible()
    {
        CursorInfo info = new();
        info.cbSize = Marshal.SizeOf(info);
        var shown = false;
        if (GetCursorInfo(out info))
        {
            shown = (info.flags & CURSOR_SHOWING) != 1;
        }
        return shown;
    }
}