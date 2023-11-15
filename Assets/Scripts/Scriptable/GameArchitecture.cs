using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    [CreateAssetMenu(fileName = "New Game Template", menuName = "Hidden Object/Game Template")]
    public class GameArchitecture : ScriptableObject
    {
        public List<ChapterTemplate> currentGameChapters;
    }
}
