using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class LevelLoader : MonoBehaviour
    {
        public List<LevelTemplate> levelTemplates = new List<LevelTemplate>();//WIP

        private LevelTemplate levelTemplate;
        public void LoadLevel(string v_levelID)
        {
            levelTemplate = Resources.Load(GameplayManager.Instance.levelLoaderTemplatesPath + v_levelID) as LevelTemplate;
            InstantiateLevel();
        }

        public void LoadLevel(int v_levelIndex)//WIP
        {
            levelTemplate = levelTemplates[v_levelIndex];
            InstantiateLevel();
        }

        private void InstantiateLevel()
        {
            GameObject levelInstance = Instantiate(levelTemplate.levelPrefab);
            Level currentLoadedLevel = levelInstance.GetComponent<Level>();
            GameplayManager.Instance.GameType = levelTemplate.gameType;
            GameplayManager.Instance.CurrentLoadedLevel = currentLoadedLevel;
            currentLoadedLevel.StartLevel(levelTemplate);
        }
    }
}
