using OwChatAssistant.Library.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OwChatAssistant.Library.Services;

[SupportedOSPlatform("windows")]
public class CursorDetector : ICursorDetector
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
    private static extern bool GetCursorInfo(ref CursorInfo pci);

    public bool IsCursorVisible()
    {
        var info = new CursorInfo { cbSize = Marshal.SizeOf<CursorInfo>() };
        return GetCursorInfo(ref info) && (info.flags & CURSOR_SHOWING) != 1;
    }
}