using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class LevelLoader : MonoBehaviour
    {
        LevelTemplate levelTemplate;
        Level currentLoadedLevel;
        public void LoadLevel(string levelID)
        {
            levelTemplate = Resources.Load("Level Instances/" + levelID) as LevelTemplate;
            GameObject levelInstance = Instantiate(levelTemplate.levelPrefab);
            currentLoadedLevel = levelInstance.GetComponent<Level>();
            GameplayManager.Instance.GameType = levelTemplate.gameType;
        }

        public Level CurrentLoadedLevel
        {
            get { return currentLoadedLevel; }
        }
    }
}
