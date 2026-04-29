using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AntiToxicOverlay
{
    public static class KeyboardHook
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;

        private static bool chatOpen = false;
        private static StringBuilder buffer = new StringBuilder();

        private static OverlayForm? overlay;
        private static List<string> toxicWords = [];

        public static void Start(OverlayForm form)
        {
            overlay = form;
            _hookID = SetHook(_proc);
            toxicWords = File.ReadAllLines("toxic_words.txt").Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
            Console.WriteLine($"Loaded {toxicWords.Count} toxic words.");
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            if (curModule == null)
                throw new Exception("MainModule is null");

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

            // 🟢 ENTER → toggle chat state
            if (key == Keys.Enter)
            {
                chatOpen = CursorDetector.IsCursorVisible();
                if (!chatOpen)
                {
                    Console.WriteLine("Analizing...");
                    string text = buffer.ToString();
                    chatOpen = AnalyzeAndHandle(text);
                    buffer.Clear();
                }
                else
                {
                    Console.WriteLine("Start Analizing...");
                    buffer.Clear();
                }

                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            // 🟡 ESC → cancel chat
            if (key == Keys.Escape && chatOpen)
            {
                chatOpen = CursorDetector.IsCursorVisible();
                buffer.Clear();

                overlay?.ShowToast("Mensaje cancelado", MessageType.Info);

                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            // 🚨 CLAVE: si no estás en chat, ignorar todo
            if (!chatOpen)
                return CallNextHookEx(_hookID, nCode, wParam, lParam);

            // ✏️ construir buffer SOLO en chat
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

        // 🧠 lógica real de envío
        private static bool AnalyzeAndHandle(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            Console.WriteLine("Text to analyze: " + text);
            if (IsToxic(text))
            {
                overlay?.ShowToast("Mensaje bloqueado", MessageType.Error);

                // 🚫 cancelación real del envío
                SendKeys.SendWait("{ESC}");
                return true;
            }
            else
            {
                overlay?.ShowToast("Mensaje enviado", MessageType.Success);
                return false;
            }
        }

        private static bool IsToxic(string text)
        {
            var isToxic=false;
            var textWords = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in toxicWords)
            {
                if (textWords.Any(w => w.Equals(word, StringComparison.OrdinalIgnoreCase)))
                {
                    isToxic=true;
                    break;
                }
            }
            Console.Write($"Is Toxic: {isToxic}");
            return isToxic;
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
}