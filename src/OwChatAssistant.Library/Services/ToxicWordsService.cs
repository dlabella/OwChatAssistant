using OwChatAssistant.Configuration;
using OwChatAssistant.Library.Common.Extensions;

namespace OwChatAssistant.Library.Services;

public class ToxicWordsService(ChatAssistantSettings configuration)
{
    public IEnumerable<string> GetAllWords() =>
        configuration.Toxicity.ToxicWords.Values
            .Distinct()
            .SelectMany(wordList => wordList)
            .Select(word => word.RemoveDiacritics().ToLower());
}
