using OwChatAssistant.Library.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OwChatAssistant.Library.Services;

[SupportedOSPlatform("windows")]
public sealed class WindowsKeyboardHookService : IKeyboardHookService, IDisposable
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private readonly LowLevelKeyboardProc _proc;
    private IntPtr _hookID = IntPtr.Zero;

    public event KeyHookEventHandler? KeyDown;

    public WindowsKeyboardHookService() => _proc = HookCallback;

    public void Start()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule
            ?? throw new InvalidOperationException("MainModule is null");
        _hookID = SetWindowsHookEx(13, _proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    public void Stop()
    {
        if (_hookID == IntPtr.Zero) return;
        UnhookWindowsHookEx(_hookID);
        _hookID = IntPtr.Zero;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        const int WM_KEYDOWN = 0x0100;
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            KeyDown?.Invoke(MapVirtualKey(Marshal.ReadInt32(lParam)));

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static HookKey MapVirtualKey(int vk) => vk switch
    {
        0x0D => HookKey.Enter,
        0x1B => HookKey.Escape,
        0x08 => HookKey.Back,
        0x20 => HookKey.Space,
        0x14 => HookKey.CapsLock,
        >= 0x41 and <= 0x5A => (HookKey)((int)HookKey.A + (vk - 0x41)),
        _ => HookKey.Unknown
    };

    public void Dispose() => Stop();

    #region Win32

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion
}
