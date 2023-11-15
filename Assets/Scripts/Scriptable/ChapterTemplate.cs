using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    [CreateAssetMenu(fileName = "New Chapter Template", menuName = "Hidden Object/Chapter Template")]
    public class ChapterTemplate : ScriptableObject
    {
        public string chapterNameID;
        public List<LevelTemplate> chapterLevels;
    }
}