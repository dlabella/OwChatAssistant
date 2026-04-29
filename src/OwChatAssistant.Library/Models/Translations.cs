using System;
using System.Collections.Generic;
using System.Text;

namespace OwChatAssistant.Library.Models
{
    public class Translations(Configuration config)
    {
        private readonly string defaultLanguage = config.DefaultLanguage ?? "en";
        private readonly Dictionary<string, Dictionary<string, string>> translations = config.Translations;
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
