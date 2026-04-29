using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OwChatAssistant.Library.Services
{
    public static class CursorProcessDetector
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(
            IntPtr hWnd,
            out RECT rect);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(
            out POINT point);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        

        // 🎯 Detectar si la ventana activa es Overwatch
        public static bool IsFocused(string processName)
        {
            IntPtr hwnd = GetForegroundWindow();

            if (hwnd == IntPtr.Zero)
                return false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainWindowHandle == hwnd)
                    {
                        return process.ProcessName
                            .Contains(
                                processName,
                                StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        // 🖱️ Detectar si el cursor está dentro de la ventana
        public static bool IsCursorInsideProcess(string processName)
        {
            IntPtr hwnd = GetForegroundWindow();

            if (hwnd == IntPtr.Zero)
                return false;

            Process? process = Process
                .GetProcesses()
                .FirstOrDefault(p =>
                {
                    try
                    {
                        return p.MainWindowHandle == hwnd &&
                               p.ProcessName.Contains(
                                   processName,
                                   StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });

            if (process == null)
                return false;

            if (!GetWindowRect(hwnd, out RECT rect))
                return false;

            if (!GetCursorPos(out POINT cursor))
                return false;

            Rectangle windowRect = new Rectangle(
                rect.Left,
                rect.Top,
                rect.Right - rect.Left,
                rect.Bottom - rect.Top);

            return windowRect.Contains(
                new Point(cursor.X, cursor.Y));
        }
    }
}
