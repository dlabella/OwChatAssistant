namespace OwChatAssistant.Configuration
{
    public class LanguageSettings
    {
        public string DefaultLanguage { get; set; } = "en";


        public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = [];
    }
}
