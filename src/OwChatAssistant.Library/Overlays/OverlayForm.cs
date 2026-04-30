using OwChatAssistant.Library.Interfaces;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OwChatAssistant.Library;

public enum MessageType { Info, Success, Warning, Error }

public class OverlayForm : Form, IOverlayForm
{
    #region Constants

    private const float FadeStep = 0.06f;
    private const float GlowStep = 0.02f;
    private const int TimerInterval = 15;
    private const int ToastDelay = 2500;
    private const int ToastPadX = 60;
    private const int ToastPadY = 30;
    private const int ToastOffsetX = 40;
    private const int ToastOffsetY = 140;
    private const int ChamferCut = 10;
    private const int FontSize = 18;

    #endregion

    #region Fields

    private string _message = string.Empty;
    private bool _visible = false;
    private float _targetOpacity = 0f;
    private float _glowPulse = 0f;
    private bool _glowIncreasing = true;
    private CancellationTokenSource? _toastCts;

    private readonly System.Windows.Forms.Timer _fadeTimer;
    private readonly Font font = new("Segoe UI Semibold", FontSize);
    #endregion

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MessageType Style { get; set; } = MessageType.Info;

    public OverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.LimeGreen;
        TransparencyKey = Color.LimeGreen;
        WindowState = FormWindowState.Maximized;
        Opacity = 0;
        DoubleBuffered = true;

        _fadeTimer = new System.Windows.Forms.Timer { Interval = TimerInterval };
        _fadeTimer.Tick += FadeTick;
    }

    // Click-through overlay
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x80000 | 0x20;
            return cp;
        }
    }

    public async Task ShowToast(string text, MessageType type = MessageType.Info)
    {
        if (_toastCts != null && !_toastCts.IsCancellationRequested)
        {
            await _toastCts.CancelAsync();
        }
        _toastCts = new CancellationTokenSource();
        var token = _toastCts.Token;

        _message = text;
        Style = type;
        _visible = true;

        SetTargetOpacity(1f);

        try
        {
            await Task.Delay(ToastDelay, token);
            SetTargetOpacity(0f);
        }
        catch (TaskCanceledException)
        {
            // Expected when toast is cancelled by a new ShowToast call
        }
    }

    private void SetTargetOpacity(float target)
    {
        _targetOpacity = target;
        _fadeTimer.Start();
    }

    private void FadeTick(object? sender, EventArgs e)
    {
        UpdateOpacity();
        UpdateGlowPulse();

        if (Opacity <= 0 && _targetOpacity == 0)
        {
            _fadeTimer.Stop();
            _visible = false;
        }

        Invalidate();
    }

    private void UpdateOpacity()
    {
        if (_targetOpacity > Opacity)
            Opacity = Math.Min(Opacity + FadeStep, _targetOpacity);
        else
            Opacity = Math.Max(Opacity - FadeStep, _targetOpacity);
    }

    private void UpdateGlowPulse()
    {
        _glowPulse += _glowIncreasing ? GlowStep : -GlowStep;

        if (_glowPulse >= 1f) { _glowPulse = 1f; _glowIncreasing = false; }
        else if (_glowPulse <= 0f) { _glowPulse = 0f; _glowIncreasing = true; }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!_visible && Opacity <= 0) return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var textSize = g.MeasureString(_message, font);
        var rect = new Rectangle(
            ToastOffsetX,
            Height - ToastOffsetY,
            (int)textSize.Width + ToastPadX,
            (int)textSize.Height + ToastPadY
        );

        DrawToastBackground(g, rect);
        DrawToastBorder(g, rect);
        DrawToastText(g, rect, font, textSize);
    }

    private static void DrawToastBackground(Graphics g, Rectangle rect)
    {
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(0xFF, 0x00, 0x71, 0xCD),
            Color.FromArgb(0xFF, 0x00, 0x4F, 0x8F),
            90f);
        FillChamfered(g, brush, rect, ChamferCut);
    }

    private void DrawToastBorder(Graphics g, Rectangle rect)
    {
        int glowAlpha = (int)(120 + _glowPulse * 100);
        using var pen = new Pen(Color.FromArgb(glowAlpha, 0, 180, 255), 2);
        DrawChamfered(g, pen, rect, ChamferCut);
    }

    private void DrawToastText(Graphics g, Rectangle rect, Font font, SizeF textSize)
    {
        using var brush = new SolidBrush(Color.White);
        float textX = rect.X + (rect.Width - textSize.Width) / 2;
        float textY = rect.Y + (rect.Height - textSize.Height) / 2;
        g.DrawString(_message, font, brush, textX, textY);
    }

    #region Chamfer Helpers

    private static void FillChamfered(Graphics g, Brush b, Rectangle r, int cut)
    {
        using var path = ChamferedRect(r, cut);
        g.FillPath(b, path);
    }

    private static void DrawChamfered(Graphics g, Pen p, Rectangle r, int cut)
    {
        using var path = ChamferedRect(r, cut);
        g.DrawPath(p, path);
    }

    private static GraphicsPath ChamferedRect(Rectangle b, int cut)
    {
        int x = b.X, y = b.Y, w = b.Width, h = b.Height;
        var path = new GraphicsPath();
        path.StartFigure();
        path.AddLines([
            new PointF(x + cut,     y),
            new PointF(x + w - cut, y),
            new PointF(x + w,       y + cut),
            new PointF(x + w,       y + h - cut),
            new PointF(x + w - cut, y + h),
            new PointF(x + cut,     y + h),
            new PointF(x,           y + h - cut),
            new PointF(x,           y + cut),
            new PointF(x + cut,     y)
        ]);
        path.CloseFigure();
        return path;
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            font.Dispose();
            _toastCts?.Cancel();
            _toastCts?.Dispose();
            _fadeTimer.Dispose();
        }
        base.Dispose(disposing);
    }
}