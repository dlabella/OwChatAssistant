using OwChatAssistant.Library;
using OwChatAssistant.Library.Services;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace OwChatAssistant
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
            var chatAssistantService = new ChatAssistantService(overlay);
            chatAssistantService.Start();

            // Menú contextual del tray
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Salir", null, (s, e) =>
            {
                Application.Exit();
            });

            // Ícono en la bandeja del sistema
            var trayIcon = new NotifyIcon
            {
                Icon = new Icon("Assets\\Icons\\app.ico"), 
                Text = "Overwatch Chat Assistant",
                Visible = true,
                ContextMenuStrip = contextMenu
            };

            trayIcon.DoubleClick += (s, e) => { Application.Exit(); };

            Application.ApplicationExit += (s, e) =>
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            };

            Application.Run(overlay);
        }

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
    }
}