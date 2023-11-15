using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThreeLittleBerkana
{
    public class LocalizationManager : StaticInstance<LocalizationManager>
    {
        public static Action OnLanguageUpdated;
        public static Action OnLocalizationLoaded;

        static string language;

        [Header("UI Localization")]
        public string UILocalizationPath;
        private Dictionary<string, string> uiLocalizationDictionary;

        private static char csvLineSeparator = '\n';
        private static char csvLineSurrounder = '"';
        private static string[] csvFieldSeparator = { "\",\"" };

        public static string LANGUAGE
        {
            get { return language; }
            set { language = value; }
        }

        public static Dictionary<string, string> GetLocalizationDictionary(string v_language, string v_csvPath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            TextAsset interfaceCsvFile = CsvLoader.LoadCSV(v_csvPath);
            string[] lines = interfaceCsvFile.text.Split(csvLineSeparator);
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

        public string GetLocalizationText(dictionaryType v_dictionaryType, string v_localizationKey)
        {
            switch (v_dictionaryType)
            {
                default:
                    return "ERROR";
                case dictionaryType.UI:
                    return uiLocalizationDictionary[v_localizationKey];
            }
        }

        private void Start()
        {
            switch (Application.systemLanguage)
            {
                default:
                case SystemLanguage.English:
                    LANGUAGE = "EN";
                    break;
                    /*case SystemLanguage.Spanish:
                        LANGUAGE = "ES";
                        break;*/
            }
            OnLanguageUpdated?.Invoke();

            uiLocalizationDictionary = GetLocalizationDictionary(LANGUAGE, UILocalizationPath);
            OnLocalizationLoaded?.Invoke();
        }
    }

    public enum dictionaryType
    {
        UI,
        HIDDEN_OBJECT,
        DIALOGUE
    };
}
