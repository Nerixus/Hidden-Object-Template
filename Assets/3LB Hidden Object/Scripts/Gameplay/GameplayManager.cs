using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class GameplayManager : StaticInstance<GameplayManager>
    {
        public LevelLoader levelLoader;
        private Level currentLoadedLevel;
        public delegate void HandleGameVictory();
        public static HandleGameVictory OnGameVictory;

        GAME_TYPE gameType;

        public GAME_TYPE GameType
        {
            get
            {
                return gameType;
            }
            set
            {
                gameType = value;
            }
        }

        public Level CurrentLoadedLevel
        {
            get { return currentLoadedLevel; }
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

        void LoadHiddenObjectLevel(string v_levelID)
        {
            levelLoader.LoadLevel(v_levelID);
            currentLoadedLevel = levelLoader.CurrentLoadedLevel;
        }

        void LoadHiddenObjectLevel(int v_levelIndex)
        {

        }

        void ProcessObjectFound(HiddenObject v_hiddenObject)
        {
            currentLoadedLevel.ProcessHiddenObjectFound(v_hiddenObject);
            ReviewGameStatus();
        }

        void ReviewGameStatus()
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

