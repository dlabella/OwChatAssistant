using OwChatAssistant.Common;
using OwChatAssistant.Configuration;
using OwChatAssistant.Library.Interfaces;

namespace OwChatAssistant.Library.Services
{
    public class ChatAssistantService
    {
        private readonly TranslationService translations;
        private readonly ToxicityAnalyzerService toxicityAnalyzerService;
        private readonly IOverlayForm overlay;
        private readonly ChatAssistantSettings? config;
        private readonly ChatHookService chatHookService;

        public ChatAssistantService(IOverlayForm overlay, IKeyboardHookService keyboard, ICursorDetector cursor)
        {
            this.overlay = overlay;
            config = ChatAssistantSettings.Load();
            translations = new TranslationService(config);
            toxicityAnalyzerService = new ToxicityAnalyzerService(new ToxicWordsService(config));
            chatHookService = new ChatHookService(keyboard, cursor, config)
            {
                OnChatMessage = AnalyzeChatMessage
            };
        }

        public void Start()
        {
            chatHookService.Start();
            overlay.ShowToast(translations.GetTranslation("ServiceStarted"), MessageType.Info);
            Logger.Log("Chat Assistant Service started.");
        }

        public void Stop()
        {
            chatHookService.Stop();
            overlay.ShowToast(translations.GetTranslation("ServiceStopped"), MessageType.Info);
            Logger.Log("Chat Assistant Service stopped.");
        }

        private bool AnalyzeChatMessage(string message)
        {
            Logger.Log($"Chat message: {message}");
            var isToxic = toxicityAnalyzerService.IsToxic(message);
            if (isToxic)
            {
                Logger.Log("Blocked toxic message");
                switch (config?.Toxicity.ToxicityBehavior)
                {
                    case ToxicityBehavior.Warn:
                        isToxic = false;
                        overlay.ShowToast(translations.GetTranslation("ToxicMessage"), MessageType.Warning);
                        break;
                    case ToxicityBehavior.Block:
                        overlay.ShowToast(translations.GetTranslation("MessageBlocked"), MessageType.Warning);
                        break;

                    case ToxicityBehavior.BlockSilent:
                        break;
                }
            }
            return !isToxic;
        }
    }
}

