using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace ThreeLittleBerkana
{
    public class LocalizationDictionary : MonoBehaviour
    {
        public bool shouldCastMessage;
        public dictionaryType dictionaryKey;

        public delegate void HandleDictionaryLoaded(dictionaryType v_type);
        public static event HandleDictionaryLoaded OnDictionaryLoaded;

        public Dictionary<string, string> currentLanguageDictionary = new Dictionary<string, string>();

        public string GetLocalizationText(string v_key)
        {
            if (currentLanguageDictionary.ContainsKey(v_key))
            {
                return currentLanguageDictionary[v_key];
            }
            else
            {
                Debug.LogWarning("No key was found for the selected language and dictionary");
                return "";
            }
        }

        public virtual void LoadLocalization()
        {

        }
    }
}

