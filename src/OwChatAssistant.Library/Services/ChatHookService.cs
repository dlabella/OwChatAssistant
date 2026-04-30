using OwChatAssistant.Common;
using OwChatAssistant.Configuration;
using OwChatAssistant.Library.Interfaces;
using System.Text;
using System.Windows.Forms;

namespace OwChatAssistant.Library.Services;

public sealed class ChatHookService
{
    private readonly IKeyboardHookService _keyboard;
    private readonly ICursorDetector _cursor;
    private readonly StringBuilder _buffer = new();
    private bool _chatOpen;
    private bool _disableCapsLock;

    public Func<string, bool> OnChatMessage { get; set; } = _ => true;

    public ChatHookService(IKeyboardHookService keyboard, ICursorDetector cursor, ChatAssistantSettings config)
    {
        _keyboard = keyboard;
        _cursor = cursor;
        _disableCapsLock = config.Keyboard.DisableBloqMayus;
        _keyboard.KeyDown += HandleKey;
    }

    public void Start() => _keyboard.Start();
    public void Stop() => _keyboard.Stop();

    private void HandleKey(HookKey key)
    {
        switch (key)
        {
            case HookKey.CapsLock when _disableCapsLock:
                break;
            case HookKey.Enter:
                HandleEnter();
                break;
            case HookKey.Escape when _chatOpen:
                _chatOpen = _cursor.IsCursorVisible();
                _buffer.Clear();
                break;
            default:
                HandleCharacter(key);
                break;
        }
    }

    private void HandleEnter()
    {
        _chatOpen = _cursor.IsCursorVisible();
        
        Logger.Log($"Chat Open: {_chatOpen}");
 
        if (!_chatOpen && _buffer.Length>0)
        {
            var text = _buffer.ToString();
            var isValid = OnChatMessage(text);
            if (!isValid) 
            {
                Logger.Log("Sending ESC to close chat");
                SendKeys.SendWait("{ESC}");
            }
            _chatOpen = !isValid;
        }
        _buffer.Clear();
    }


    private void HandleCharacter(HookKey key)
    {
        if (!_chatOpen) return;

        switch (key)
        {
            case HookKey.Back when _buffer.Length > 0:
                _buffer.Length--;
                break;
            case HookKey.Space:
                _buffer.Append(' ');
                break;
            default:
                if (key is >= HookKey.A and <= HookKey.Z)
                    _buffer.Append(key.ToString().ToLower());
                break;
        }
    }
}

