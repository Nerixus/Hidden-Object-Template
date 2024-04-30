// Copyright (c) Pixel Crushers. All rights reserved.

#if USE_OPENAI

using System.Globalization;
using System.Text.RegularExpressions;

namespace PixelCrushers.DialogueSystem.OpenAIAddon
{

    /// <summary>
    /// Utility methods for OpenAI addon.
    /// </summary>
    public static class AITextUtility
    {

        /// <summary>
        /// Gets English language name from a language code. 
        /// If can't determine English name, returns language code itself.
        /// </summary>
        public static string DetermineLanguage(string languageCode)
        {
            var cultureInfo = new CultureInfo(languageCode);
            return (cultureInfo == null || string.IsNullOrEmpty(cultureInfo.Name)) ? languageCode : cultureInfo.EnglishName;
        }

        /// <summary>
        /// Removes "speaker:", "speaker says:", or surrounding quotes around lines.
        /// </summary>
        public static string RemoveSpeaker(string speaker, string line)
        {
            if (line.StartsWith($"{speaker}:"))
            {
                return RemoveSurroundingQuotes(line.Substring(speaker.Length + 2));
            }
            else
            {
                // Match:
                // <speaker> <says>: text
                // where <says> could be any word and text could have surrounding quotes.
                var pattern = $"^{speaker} \\w+[:,] ";
                var match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    return RemoveSurroundingQuotes(line.Substring(match.Value.Length));
                }
                // Otherwise if line ends with a quote character,
                // grab text from first quote to end.
                else if (line.EndsWith('"'))
                {
                    var pos = line.IndexOf('"');
                    return RemoveSurroundingQuotes(line.Substring(pos));
                }
                else if (line.EndsWith("'"))
                {
                    var pos = line.IndexOf(" '");
                    return RemoveSurroundingQuotes(line.Substring(pos + 1));
                }
                // Otherwise look for <speaker> says, text
                else if (line.Contains(" says, "))
                {
                    var pos = line.IndexOf(" says, ");
                    return RemoveSurroundingQuotes(line.Substring(pos + " says, ".Length));
                }
                else
                {
                    return line;
                }
            }
        }

        /// <summary>
        /// Removes double quotes around string if present.
        /// </summary>
        public static string RemoveSurroundingQuotes(string text)
        {
            if (text.StartsWith('"') || text.StartsWith('\''))
            {
                return text.Substring(1, text.Length - 2);
            }
            else
            {
                return text;
            }
        }

        public static string DoubleQuotesToSingle(string text)
        {
            return text.Replace('"', '\'');
        }

    }

}

#endif
