using OwChatAssistant.Library.Models;

namespace OwChatAssistant.Library.Services
{
    public class ToxicityAnalyzerService(ToxicWords toxicWords)
    {
        private readonly IEnumerable<string> words = toxicWords.GetAllWords();

        public bool IsToxic(string text)
        {
            var textWords = text.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':', '-', '_', '(', ')', '[', ']', '{', '}', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Any(x => textWords.Contains(x, StringComparer.OrdinalIgnoreCase));
        }
    }
}
