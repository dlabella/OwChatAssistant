using OwChatAssistant.Library.Common.Extensions;

namespace OwChatAssistant.Library.Services;

public class ToxicityAnalyzerService(ToxicWordsService toxicWords)
{
    private static readonly char[] _separators = [' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':', '-', '_', '(', ')', '[', ']', '{', '}', '"', '\''];

    // ToxicWordsService already returns words normalized (lowercased, no diacritics)
    private readonly HashSet<string> _words = [.. toxicWords.GetAllWords()];

    public bool IsToxic(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        var normalized = text.RemoveDiacritics().ToLower();
        var textWords = normalized.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        return Array.Exists(textWords, w => _words.Contains(w));
    }
}
