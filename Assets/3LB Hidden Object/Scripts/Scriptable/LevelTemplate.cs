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
        public DISPLAY_MODE displayMode;
        [Range(1,9)] public int simultaneousObjectsToFind = 3;
        public TextAsset localizationFile;
        public HIDDEN_OBJECT_EXIT_MODE foundFeedback;
        public GameObject foundParticles;
    }
}

