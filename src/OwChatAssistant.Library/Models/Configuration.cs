using System;
using System.Collections.Generic;
using System.Text;

namespace OwChatAssistant.Library.Models
{
    using System.Collections.Generic;

    public class Configuration
    {
        public string DefaultLanguage { get; set; } = "en";
        public ToxicityBehavior ToxicityBehavior { get; set; } = ToxicityBehavior.Warn;

        public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = [];

        public Dictionary<string, List<string>> ToxicWords { get; set; } = [];
    }
    public enum ToxicityBehavior
    {
        Block,
        Warn,
        BlockSilent
    }
}
