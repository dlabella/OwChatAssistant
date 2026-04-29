using System.Globalization;
using System.Text;

namespace OwChatAssistant.Library.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            ).Normalize(NormalizationForm.FormC);
        }
    }
}