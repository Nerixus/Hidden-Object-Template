using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    [CreateAssetMenu(fileName = "New Level Template", menuName = "Hidden Object/Level Template")]
    public class LevelTemplate : ScriptableObject
    {
        public GameObject levelPrefab;
        public GAME_TYPE gameType;
        //public string levelUniqueID;
        //public string chapterLevelID;
        //public string levelName;
        //public int levelIndex;
        //public int levelCost = 15;
        //public Sprite levelSprite;
        //public List<ExpertiseLevelData> expertiseLevels;
    }

    [System.Serializable]
    public class ExpertiseLevelData
    {
        public int startingExperience;
        public int experienceRequired;
        public bool shouldUnlock;
        public string unlockKey;
        public Sprite unlockSprite;
        public string clueDisplayName;
        public string startingDialogueID;
        public string endingDialogueID;

        public bool IsInRange(int value)
        {
            if (value < experienceRequired && value >= startingExperience)
                return true;
            else
                return false;
        }

        public float GetNormalizedProgress(int v_value)
        {
            float normalizedCeil = (float)experienceRequired - (float)startingExperience;
            float normalizedProgress = (float)v_value - (float)startingExperience;
            float returnValue = normalizedProgress / normalizedCeil;

            return returnValue;
        }
    }
}

