using OwChatAssistant.Library.Exceptions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace OwChatAssistant.Library.Services;

public static class KeyboardHookService
{
    private static readonly LowLevelKeyboardProc _proc = HookCallback;
    private static readonly StringBuilder buffer = new();

    private static bool chatOpen = false;
    private static IntPtr _hookID = IntPtr.Zero;

    public static Func<string, bool> OnChatMessage { get; set; } = (x) => true;
    public static void Start()
    {
        _hookID = SetHook(_proc);
    }
    public static void Stop()
    {
        if (_hookID == IntPtr.Zero)
            return;

        UnhookWindowsHookEx(_hookID);

        _hookID = IntPtr.Zero;
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule ?? throw new ProgramException("MainModule is null");

        return SetWindowsHookEx(
            13,
            proc,
            GetModuleHandle(curModule.ModuleName),
            0
        );
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        if (wParam != (IntPtr)0x0100) // WM_KEYDOWN
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        int vkCode = Marshal.ReadInt32(lParam);
        Keys key = (Keys)vkCode;

        if (key == Keys.Enter)
        {
            Logger.Log("Enter key pressed. Chat open: " + chatOpen);
            chatOpen = CursorDetector.IsCursorVisible();
            if (!chatOpen)
            {
                string text = buffer.ToString();
                var isValidMessage = OnChatMessage(text);
                if (!isValidMessage)
                {
                    SendKeys.SendWait("{ESC}");
                }
                chatOpen = !isValidMessage;
            }
            buffer.Clear();
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        if (key == Keys.Escape && chatOpen)
        {
            chatOpen = CursorDetector.IsCursorVisible();
            buffer.Clear();
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        if (!chatOpen)
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        if (key == Keys.Back)
        {
            if (buffer.Length > 0)
                buffer.Length--;
        }
        else if (key == Keys.Space)
        {
            buffer.Append(" ");
        }
        else if (key >= Keys.A && key <= Keys.Z)
        {
            buffer.Append(key.ToString().ToLower());
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }


    #region WinAPI

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
        IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion
}
