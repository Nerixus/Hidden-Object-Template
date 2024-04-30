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
        private DISPLAY_MODE displayMode;

        public delegate void HandleGameVictory();
        public static HandleGameVictory OnGameVictory;

        public GAME_TYPE GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }

        public DISPLAY_MODE DisplayMode
        {
            get { return displayMode; }
            set { displayMode = value; }
        }

        public Level CurrentLoadedLevel
        {
            get { return currentLoadedLevel; }
            set { currentLoadedLevel = value; }
        }

        private void OnEnable()
        {
            HiddenObject.OnObjectFound += ProcessObjectFound;
        }

        private void OnDisable()
        {
            HiddenObject.OnObjectFound -= ProcessObjectFound;
        }

        public void LoadHiddenObjectLevel(string v_levelID, string v_language)
        {
            language = v_language;
            levelLoader.LoadLevel(v_levelID);
        }

        public void LoadHiddenObjectLevel(int v_index, string v_language)
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
        TWO_D_UI
    }
}

