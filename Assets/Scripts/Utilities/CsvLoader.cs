using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class CsvLoader
    {
        public static TextAsset LoadCSV(string v_csvName)
        {
            TextAsset csvFile;
            csvFile = Resources.Load<TextAsset>(v_csvName);
            return csvFile;
        }
    }
}
