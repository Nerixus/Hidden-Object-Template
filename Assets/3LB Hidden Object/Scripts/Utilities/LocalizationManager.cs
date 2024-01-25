using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThreeLittleBerkana
{
    public class LocalizationManager : StaticInstance<LocalizationManager>
    {
        private static char csvLineSeparator = '\n';
        private static char csvLineSurrounder = '"';
        private static string[] csvFieldSeparator = { "\",\"" };

        public static Dictionary<string, string> GetLocalizationDictionary(string v_language, TextAsset csvFile)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] lines = csvFile.text.Split(csvLineSeparator);
            int attributeIndex = -1;
            string[] headers = lines[0].Split(csvFieldSeparator, StringSplitOptions.None);

            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains(v_language))
                {
                    attributeIndex = i;
                    break;
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = line.Split(csvFieldSeparator, StringSplitOptions.None);

                for (int j = 0; j < fields.Length; j++)
                {
                    fields[j] = fields[j].TrimStart(' ', csvLineSurrounder);
                    fields[j] = fields[j].TrimEnd(csvLineSurrounder, '\r', '\n');
                }

                if (fields.Length > attributeIndex)
                {
                    var key = fields[0];
                    if (dictionary.ContainsKey(key)) { continue; }

                    dictionary.Add(key, fields[attributeIndex]);
                }
            }
            return dictionary;
        }
    }
}
