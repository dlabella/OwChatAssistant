namespace OwChatAssistant.Library.Interfaces
{
    public interface IOverlayForm
    {
        MessageType Style { get; set; }

        Task ShowToast(string text, MessageType type = MessageType.Info);
    }
}