using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AntiToxicOverlay
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            #if DEBUG
            AllocConsole();
            #endif
            ApplicationConfiguration.Initialize();

            var overlay = new OverlayForm();
            KeyboardHook.Start(overlay);

            Application.Run(overlay);
        }

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
    }
}