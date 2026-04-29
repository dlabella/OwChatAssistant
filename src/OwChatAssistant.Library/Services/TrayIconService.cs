using System.Drawing.Drawing2D;

namespace OwChatAssistant.Library.Services
{
    public class TrayIconService
    {
        private readonly Icon _playIcon;
        private readonly Icon _pauseIcon;
        private readonly ChatAssistantService _chatAssistantService;
        private NotifyIcon? _trayIcon;
        public TrayIconService(ChatAssistantService chatAssistantService)
        {
            _chatAssistantService = chatAssistantService;
            var baseIcon = new Icon("Assets\\Icons\\app.ico");
            _playIcon = CreateOverlayIcon(baseIcon, true);
            _pauseIcon = CreateOverlayIcon(baseIcon, false);
            Application.ApplicationExit += (s, e) =>
            {
                _trayIcon?.Visible = false;
                _trayIcon?.Dispose();
            };

        }
        public void AddTrayIcon()
        {
            // Menú contextual del tray
            var contextMenu = new ContextMenuStrip();
            var trayIcon = new NotifyIcon
            {
                Icon = _pauseIcon,
                Text = "Overwatch Chat Assistant",
                Visible = true,
                ContextMenuStrip = contextMenu
            };
            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                Application.Exit();
            });
            contextMenu.Items.Add("Start", null, (s, e) =>
            {
                trayIcon.Icon = _playIcon;
                _chatAssistantService.Start();
            });
            contextMenu.Items.Add("Stop", null, (s, e) =>
            {
                trayIcon.Icon = _pauseIcon;
                _chatAssistantService.Stop();
            });
            trayIcon.DoubleClick += (s, e) => { Application.Exit(); };
            _trayIcon = trayIcon;
        }

        private static Icon CreateOverlayIcon(Icon baseIcon, bool play)
        {
            const int size = 64;

            var bitmap = new Bitmap(size, size);

            using var graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.Clear(Color.Transparent);

            graphics.DrawIcon(baseIcon, new Rectangle(0, 0, size, size));

            int badgeSize = 34;
            int margin = 0;

            int x = size - badgeSize - margin;
            int y = size - badgeSize - margin;

            // Fondo negro semitransparente
            using var badgeBrush = new SolidBrush(Color.FromArgb(240, 0, 0, 0));
            graphics.FillEllipse(badgeBrush, x, y, badgeSize, badgeSize);

            // Borde blanco para destacar
            using var borderPen = new Pen(Color.White, 2);
            graphics.DrawEllipse(borderPen, x, y, badgeSize, badgeSize);

            using var symbolBrush = new SolidBrush(Color.White);

            if (play)
            {
                PointF[] triangle =
                [
                    new PointF(x + 11, y + 8),
                    new PointF(x + 11, y + 26),
                    new PointF(x + 26, y + 17)
                ];

                graphics.FillPolygon(symbolBrush, triangle);
            }
            else
            {
                graphics.FillRectangle(symbolBrush, x + 9, y + 7, 5, 20);
                graphics.FillRectangle(symbolBrush, x + 19, y + 7, 5, 20);
            }

            return Icon.FromHandle(bitmap.GetHicon());
        }

    }
}