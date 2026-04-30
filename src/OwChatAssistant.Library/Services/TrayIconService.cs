using System.Drawing.Drawing2D;

namespace OwChatAssistant.Library.Services;

public class TrayIconService : IDisposable
{
    private const int IconSize = 64;
    private const int BadgeSize = 34;
    private const string AppName = "Overwatch Chat Assistant";

    private readonly Icon _playIcon;
    private readonly Icon _pauseIcon;
    private readonly ChatAssistantService _chatAssistantService;
    private NotifyIcon? _trayIcon;
    private bool _disposed;

    public TrayIconService(ChatAssistantService chatAssistantService)
    {
        _chatAssistantService = chatAssistantService;

        using var baseIcon = new Icon("Assets\\Icons\\app.ico");
        _playIcon = CreateOverlayIcon(baseIcon, play: true);
        _pauseIcon = CreateOverlayIcon(baseIcon, play: false);

        Application.ApplicationExit += OnApplicationExit;
    }

    public void AddTrayIcon()
    {
        var contextMenu = new ContextMenuStrip();
        _trayIcon = new NotifyIcon
        {
            Icon = _pauseIcon,
            Text = AppName,
            Visible = true,
            ContextMenuStrip = contextMenu
        };

        contextMenu.Items.Add("Exit", null, (_, _) => Application.Exit());
        contextMenu.Items.Add("Start", null, (_, _) => OnStart());
        contextMenu.Items.Add("Stop", null, (_, _) => OnStop());

        _trayIcon.DoubleClick += (_, _) => Application.Exit();
    }

    private void OnStart()
    {
        if (_trayIcon is null) return;
        _trayIcon.Icon = _playIcon;
        _chatAssistantService.Start();
    }

    private void OnStop()
    {
        if (_trayIcon is null) return;
        _trayIcon.Icon = _pauseIcon;
        _chatAssistantService.Stop();
    }

    private void OnApplicationExit(object? sender, EventArgs e) => Dispose();

    private static Icon CreateOverlayIcon(Icon baseIcon, bool play)
    {
        using var bitmap = new Bitmap(IconSize, IconSize);
        using var g = Graphics.FromImage(bitmap);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(Color.Transparent);
        g.DrawIcon(baseIcon, new Rectangle(0, 0, IconSize, IconSize));

        int x = IconSize - BadgeSize;
        int y = IconSize - BadgeSize;

        DrawBadge(x, y, g);

        using var symbolBrush = new SolidBrush(Color.White);

        if (play)
            DrawPlayIcon(x, y, g, symbolBrush);
        else
        {
            DrawPauseIcon(x, y, g, symbolBrush);
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static void DrawBadge(int x, int y, Graphics g)
    {
        using var badgeBrush = new SolidBrush(Color.FromArgb(240, 0, 0, 0));
        g.FillEllipse(badgeBrush, x, y, BadgeSize, BadgeSize);

        using var borderPen = new Pen(Color.White, 2);
        g.DrawEllipse(borderPen, x, y, BadgeSize, BadgeSize);
    }

    private static void DrawPlayIcon(int x, int y, Graphics g, SolidBrush brush)
    {
        g.FillPolygon(brush, (PointF[])[
                new(x + 11, y + 8),
                new(x + 11, y + 26),
                new(x + 26, y + 17)
            ]);
    }
    private static void DrawPauseIcon(int x, int y, Graphics g, SolidBrush brush)
    {
        g.FillRectangle(brush, x + 9, y + 7, 5, 20);
        g.FillRectangle(brush, x + 19, y + 7, 5, 20);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            Application.ApplicationExit -= OnApplicationExit;
            _trayIcon?.Dispose();
            _playIcon?.Dispose();
            _pauseIcon?.Dispose();
        }

        // Unmanaged resources are cleaned up by Icon.Dispose()
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TrayIconService()
    {
        Dispose(false);
    }
}