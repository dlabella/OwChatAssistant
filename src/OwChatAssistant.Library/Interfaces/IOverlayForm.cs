namespace OwChatAssistant.Library.Interfaces
{
    public interface IOverlayForm
    {
        MessageType Style { get; set; }

        void ShowToast(string text, MessageType type = MessageType.Info);
    }
}