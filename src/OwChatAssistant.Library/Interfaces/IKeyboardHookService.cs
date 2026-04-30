namespace OwChatAssistant.Library.Interfaces;

public interface IKeyboardHookService
{
    event KeyHookEventHandler? KeyDown;
    void Start();
    void Stop();
}

public delegate void KeyHookEventHandler(HookKey key);

public enum HookKey
{
    Unknown,
    Enter, Escape, Back, Space, CapsLock,
    A, B, C, D, E, F, G, H, I, J, K, L, M,
    N, O, P, Q, R, S, T, U, V, W, X, Y, Z
}
