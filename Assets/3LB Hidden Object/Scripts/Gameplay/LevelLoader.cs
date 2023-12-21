using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class LevelLoader : MonoBehaviour
    {
        public List<LevelTemplate> levelTemplates = new List<LevelTemplate>();
        private LevelTemplate levelTemplate;
        private Level currentLoadedLevel;
        public void LoadLevel(string v_levelID)
        {
            levelTemplate = Resources.Load("Level Instances/" + v_levelID) as LevelTemplate;
            GameObject levelInstance = Instantiate(levelTemplate.levelPrefab);
            currentLoadedLevel = levelInstance.GetComponent<Level>();
            GameplayManager.Instance.GameType = levelTemplate.gameType;
        }

        public void LoadLevel(int v_levelIndex)
        {
            levelTemplate = levelTemplates[v_levelIndex];
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
