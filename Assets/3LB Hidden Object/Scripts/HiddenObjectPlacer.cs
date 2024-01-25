using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThreeLittleBerkana
{
    public class HiddenObjectPlacer : MonoBehaviour
    {
        public Level levelToPosition;
        public TextAsset positioningCsvFile;
        public GameObject referenceObject;

        private char csvLineSeparator = '\n';
        private char csvLineSurrounder = '"';
        private string[] csvFieldSeparator = { "\",\"" };

        public Dictionary<string, Vector2> objectPositions;

        private void Start()
        {
            if (levelToPosition == null)
                levelToPosition.GetComponent<Level>();
            FillDictionaryValues();
            PlaceObjects();
        }

        public void FillDictionaryValues()
        {
            objectPositions = new Dictionary<string, Vector2>();
            string[] lines = positioningCsvFile.text.Split(csvLineSeparator);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = line.Split(csvFieldSeparator, StringSplitOptions.None);

                for (int j = 0; j < fields.Length; j++)
                {
                    fields[j] = fields[j].TrimStart(' ', csvLineSurrounder);
                    fields[j] = fields[j].TrimEnd(csvLineSurrounder, '\r', '\n');
                }
                if (objectPositions.ContainsKey(fields[0])) { continue; }
                objectPositions.Add(fields[0], new Vector2(float.Parse(fields[1]), float.Parse(fields[2])));
            }
        }

        public void PlaceObjects()
        {
            foreach (HiddenObject HO in levelToPosition.levelHiddenObjects)
            {
                if (objectPositions.ContainsKey(HO.gameObject.name))
                {
                    RectTransform rectTransform = HO.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector3(objectPositions[HO.gameObject.name].x, objectPositions[HO.gameObject.name].y, 0);
                }
            }
        }
    }
}
