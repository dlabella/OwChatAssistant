using OwChatAssistant.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OwChatAssistant.Configuration
{
    public class ChatAssistantSettings
    {
        private const string ConfigPath = "Configuration/config.json";
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
                {
                new JsonStringEnumConverter()
                }
        };

        public KeyboardSettings Keyboard { get; set; } = new();
        public ToxicitySettings Toxicity { get; set; } = new();
        public LanguageSettings Language { get; set; } = new();
        public SystemSettings System { get; set; } = new();
        public static ChatAssistantSettings Load()
        {
            Logger.Log("Loading configuration");
            var configText = string.Empty;
            ChatAssistantSettings? settings = null;
            if (File.Exists(ConfigPath))
            {
                configText =File.ReadAllText(ConfigPath);
                settings = JsonSerializer.Deserialize<ChatAssistantSettings>(configText, serializerOptions);
            }
            if (settings == null)
            {
                var defaultSettings = GetDefaultSettings();
                var defaultConfigText = JsonSerializer.Serialize(defaultSettings, serializerOptions);
                File.WriteAllText(ConfigPath, defaultConfigText);
                settings = defaultSettings;
            }
            return settings;
        }

        private static ChatAssistantSettings GetDefaultSettings()
        {
            var settings = new ChatAssistantSettings
            {
                Language = new LanguageSettings()
                {
                    DefaultLanguage = "en",
                    Translations = []
                },
                System = new SystemSettings()
                {
                    DebugMode = false
                },
                Keyboard = new KeyboardSettings()
                {
                    DisableBloqMayus = true
                },
                Toxicity = new ToxicitySettings()
                {
                    ToxicityBehavior = ToxicityBehavior.Warn,
                    ToxicWords = new Dictionary<string, List<string>>()
                    {
                        {"en", new List<string>(){"troll", "diff"}},
                        {"es", new List<string>(){"troll", "diff"}}
                    }
                }
            };
            return settings;
        }
    }
}