using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class GameplayManager : StaticInstance<GameplayManager>
    {
        public LevelLoader levelLoader;
        public string levelLoaderTemplatesPath = "Level Instances/";
        
        public string LANGUAGE
        {
            get { return language; }
        }

        private string language;
        private Level currentLoadedLevel;
        private GAME_TYPE gameType;

        public delegate void HandleGameVictory();
        public static HandleGameVictory OnGameVictory;

        public GAME_TYPE GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }

        public Level CurrentLoadedLevel
        {
            get { return currentLoadedLevel; }
            set { currentLoadedLevel = value; }
        }

        private void OnEnable()
        {
            DemoHiddenObject.OnLevelLoaded += LoadHiddenObjectLevel; //You should subscribe your own method when you wish to start the level
            HiddenObject.OnObjectFound += ProcessObjectFound;
        }

        private void OnDisable()
        {
            DemoHiddenObject.OnLevelLoaded -= LoadHiddenObjectLevel;
            HiddenObject.OnObjectFound -= ProcessObjectFound;
        }

        private void LoadHiddenObjectLevel(string v_levelID, string v_language)
        {
            language = v_language;
            levelLoader.LoadLevel(v_levelID);
        }

        private void LoadHiddenObjectLevel(int v_index, string v_language)
        {
            language = v_language;
            levelLoader.LoadLevel(v_index);
        }

        private void ProcessObjectFound(HiddenObject v_hiddenObject)
        {
            currentLoadedLevel.ProcessHiddenObjectFound(v_hiddenObject);
            ReviewGameStatus();
        }

        private void ReviewGameStatus()
        {
            if (currentLoadedLevel.IsHiddenObjectListEmpty())
                OnGameVictory?.Invoke();
        }
    }

    public enum GAME_TYPE
    {
        TWO_D_SPRITE,
        THREE_D,
        TWO_D_UI_WIP
    }
}

