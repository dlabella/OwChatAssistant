namespace OwChatAssistant.Library.Models
{
    public class ToxicWords(Configuration configuration)
    {
        public IEnumerable<string> GetAllWords()
        {
            foreach (var wordList in configuration.ToxicWords.Values)
            {
                
                foreach (var word in wordList)
                {
                    yield return word;
                }
            }
        }
    }
}
