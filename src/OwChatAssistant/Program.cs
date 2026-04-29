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
            ApplicationConfiguration.Initialize();

            var overlay = new OverlayForm();
            var chatAssistantService = new ChatAssistantService(overlay);
            var trayIconService = new TrayIconService(chatAssistantService);
            trayIconService.AddTrayIcon();

            
            Application.Run(overlay);
        }

        
    }
}