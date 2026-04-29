using OwChatAssistant.Library.Interfaces;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OwChatAssistant.Library;
public enum MessageType
{
    Info,
    Success,
    Warning,
    Error
}

public class OverlayForm : Form, IOverlayForm
{
    private string message = "";
    private bool visible = false;
    private bool chatDetected = false;
    private readonly System.Windows.Forms.Timer fadeTimer;
    private float targetOpacity = 0f;
    private const float fadeStep = 0.06f;

    private float glowPulse = 0f;
    private bool glowIncreasing = true;

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

        fadeTimer = new System.Windows.Forms.Timer();
        fadeTimer.Interval = 15;
        fadeTimer.Tick += FadeTick;
    }

    // Click-through overlay
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x80000;
            cp.ExStyle |= 0x20;
            return cp;
        }
    }

    // API pública
    public void SetChatDetected(bool detected)
    {
        chatDetected = detected;
        Invalidate();
    }
    public async void ShowToast(string text, MessageType type = MessageType.Info)
    {
        
        message = text;
        Style = type;
        visible = true;

        targetOpacity = 1f;
        fadeTimer.Start();

        await Task.Delay(2500);

        targetOpacity = 0f;
        fadeTimer.Start();
    }

    // Animación fade + glow
    private void FadeTick(object? sender, EventArgs e)
    {
        // fade
        if (targetOpacity > Opacity)
        {
            Opacity += fadeStep;
            if (Opacity >= targetOpacity)
                Opacity = targetOpacity;
        }
        else
        {
            Opacity -= fadeStep;
            if (Opacity <= targetOpacity)
            {
                Opacity = targetOpacity;

                if (Opacity <= 0)
                    visible = false;
            }
        }

        // glow pulse
        if (glowIncreasing)
        {
            glowPulse += 0.02f;
            if (glowPulse >= 1f) glowIncreasing = false;
        }
        else
        {
            glowPulse -= 0.02f;
            if (glowPulse <= 0f) glowIncreasing = true;
        }

        if (Opacity <= 0 && targetOpacity == 0)
            fadeTimer.Stop();

        Invalidate();
    }

    // Render HUD
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!visible && Opacity <= 0)
            return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var font = new Font("Segoe UI Semibold", 18);

        SizeF textSize = g.MeasureString(message, font);

        float x = 40;
        float y = Height - 140;

        Rectangle rect = new Rectangle(
            (int)x,
            (int)y,
            (int)textSize.Width + 60,
            (int)textSize.Height + 30
        );

        using (var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(0xFF, 0x00, 0x71, 0xCD),
            Color.FromArgb(0xFF, 0x00, 0x4F, 0x8F),
            90f))
        {
            FillChamfered(g, brush, rect, 10);
        }

        int glowAlpha = (int)(120 + glowPulse * 100);

        using (var pen = new Pen(Color.FromArgb(glowAlpha, 0, 180, 255), 2))
        {
            DrawChamfered(g, pen, rect, 10);
        }

        using var textBrush = new SolidBrush(Color.White);

        float textX = rect.X + (rect.Width - textSize.Width) / 2;
        float textY = rect.Y + (rect.Height - textSize.Height) / 2;

        g.DrawString(message, font, textBrush, textX, textY);
    }
    
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

    private static GraphicsPath ChamferedRect(Rectangle bounds, int cut)
    {
        var path = new GraphicsPath();

        int x = bounds.X;
        int y = bounds.Y;
        int w = bounds.Width;
        int h = bounds.Height;

        path.StartFigure();

        path.AddLine(x + cut, y, x + w - cut, y);
        path.AddLine(x + w - cut, y, x + w, y + cut);

        path.AddLine(x + w, y + cut, x + w, y + h - cut);
        path.AddLine(x + w, y + h - cut, x + w - cut, y + h);

        path.AddLine(x + w - cut, y + h, x + cut, y + h);
        path.AddLine(x + cut, y + h, x, y + h - cut);

        path.AddLine(x, y + h - cut, x, y + cut);
        path.AddLine(x, y + cut, x + cut, y);

        path.CloseFigure();

        return path;
    }
}