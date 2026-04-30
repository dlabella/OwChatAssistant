using OwChatAssistant.Common;
using OwChatAssistant.Configuration;
using OwChatAssistant.Library;
using OwChatAssistant.Library.Interfaces;
using OwChatAssistant.Library.Services;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace OwChatAssistant
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private static void EnableDebugConsole()
        {
            AllocConsole();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var stdout = new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(stdout);
            var stderr = new System.IO.StreamWriter(Console.OpenStandardError()) { AutoFlush = true };
            Console.SetError(stderr);
        }

        [STAThread]
        [SupportedOSPlatform("windows6.1")]
        static void Main()
        {
            var config = ChatAssistantSettings.Load();

            if (config.System.DebugMode)
            {
                EnableDebugConsole();
            }
            Logger.Log("App Starting...");

            ApplicationConfiguration.Initialize();

            IKeyboardHookService keyboard = CreateKeyboardHook();
            ICursorDetector cursor = CreateCursorDetector();

            var overlay = new OverlayForm();
            var chatAssistantService = new ChatAssistantService(overlay, keyboard, cursor);
            var trayIconService = new TrayIconService(chatAssistantService);
            trayIconService.AddTrayIcon();
            Logger.Log("App Running...");
            Application.Run(overlay);
        }

        private static IKeyboardHookService CreateKeyboardHook()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsKeyboardHookService();

            throw new PlatformNotSupportedException($"No keyboard hook implementation for {RuntimeInformation.OSDescription}");
        }

        private static ICursorDetector CreateCursorDetector()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new CursorDetector();

            throw new PlatformNotSupportedException($"No cursor detector implementation for {RuntimeInformation.OSDescription}");
        }
    }
}
