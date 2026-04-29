using OwChatAssistant.Library.Exceptions;
using OwChatAssistant.Library.Interfaces;
using OwChatAssistant.Library.Models;
using System.Text.Json;

namespace OwChatAssistant.Library.Services
{
    public class ChatAssistantService
    {
        private readonly Translations translations;
        private readonly ToxicityAnalyzerService toxicityAnalyzerService;
        private readonly IOverlayForm overlay;
        private readonly ProcessWatcher? serviceWatcher;
        public ChatAssistantService(IOverlayForm overlay)
        {
            this.overlay = overlay;
            var configText = File.ReadAllText("Configuration/config.json");
            if (String.IsNullOrEmpty(configText))
            {
                throw new ProgramException("Configuration file is empty or missing.");
            }
            var config = JsonSerializer.Deserialize<Configuration>(configText, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            if (config == null)
            {
                throw new ProgramException("Invalid configuration file format.");
            }
            translations = new Translations(config);
            toxicityAnalyzerService = new ToxicityAnalyzerService(new ToxicWords(config));
            AttachEvents();
            //serviceWatcher = new ProcessWatcher();
            
        }

        public void Start()
        {
            KeyboardHookService.Start();
            overlay.ShowToast(translations.GetTranslation("ServiceStarted"), MessageType.Info);
        }
        
        public void Stop()
        {
            KeyboardHookService.Stop();
            overlay.ShowToast(translations.GetTranslation("ServiceStopped"), MessageType.Info);
        }

        private void AttachEvents()
        {
            KeyboardHookService.OnChatMessage = AnalyzeChatMessage;
        }

        private bool AnalyzeChatMessage(string message)
        {
            var isToxic = toxicityAnalyzerService.IsToxic(message);
            if (isToxic)
            {
                Logger.Log("Blocked toxic message: " + message);

                overlay.ShowToast(translations.GetTranslation("MessageBlocked"), MessageType.Warning);
            }
            return !isToxic;
        }
    }
}
