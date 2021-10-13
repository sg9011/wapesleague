using System.Collections.Generic;
using System.Globalization;

namespace WaPesLeague.Constants
{
    public static class DictionaryExtensions
    {
        public static string GetValueForLanguage(this Dictionary<string, string> dict)
        {
            var upperLang = CultureInfo.CurrentCulture.Name.ToUpper() ?? Bot.SupportedLanguages.English;
            if (!dict.ContainsKey(upperLang)) {
                upperLang = Bot.SupportedLanguages.English;
            }
            return dict[upperLang];
        }
    }
}
