using OwChatAssistant.Configuration;

namespace OwChatAssistant.Library.Services
{
    public class TranslationService(ChatAssistantSettings config)
    {
        private readonly string defaultLanguage = config.Language.DefaultLanguage ?? "en";
        private readonly Dictionary<string, Dictionary<string, string>> translations = config.Language.Translations;
        public string GetTranslation(string language, string key)
        {
            if (translations.TryGetValue(key, out var langTranslations) &&
                langTranslations.TryGetValue(language, out string? translation))
            {
                return translation;
            }
            return key;
        }
        public string GetTranslation(string key)
        {
            return GetTranslation(defaultLanguage, key);
        }
    }
}
