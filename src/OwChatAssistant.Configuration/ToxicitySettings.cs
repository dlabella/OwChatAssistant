namespace OwChatAssistant.Configuration
{
    public class ToxicitySettings
    {
        public Dictionary<string, List<string>> ToxicWords { get; set; } = [];
        public ToxicityBehavior ToxicityBehavior { get; set; } = ToxicityBehavior.Warn;
    }
}
