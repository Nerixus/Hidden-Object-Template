using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class GameplayManager : StaticInstance<GameplayManager>
    {
        public LevelLoader levelLoader;
        Level currentLoadedLevel;

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

        private void OnEnable()
        {
            DemoHiddenObject.OnLevelLoaded += LoadHiddenObjectLevel;
        }

        private void OnDisable()
        {
            DemoHiddenObject.OnLevelLoaded -= LoadHiddenObjectLevel;
        }

        void LoadHiddenObjectLevel(string levelID)
        {
            levelLoader.LoadLevel(levelID);
            currentLoadedLevel = levelLoader.CurrentLoadedLevel;
        }
    }

    public enum GAME_TYPE
    {
        UI,
        TWO_D,
        THREE_D
    }
}

