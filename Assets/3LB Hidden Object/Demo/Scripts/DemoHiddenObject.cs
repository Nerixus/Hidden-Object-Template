using UnityEngine;

public class DemoHiddenObject : MonoBehaviour
{
    public string levelID;
    public bool useIndexToLoad;
    public int levelIndex;
    public string language;

    public delegate void HandleLevelLoaded(string v_levelID, string v_language);
    public static HandleLevelLoaded OnLevelLoaded;
    public delegate void HandleLevelLoadedIndex(int v_levelIndex, string v_language);
    public static HandleLevelLoadedIndex OnLevelLoadedIndex;
    
    void Start()
    {
        if (useIndexToLoad)
            OnLevelLoadedIndex?.Invoke(levelIndex, language);
        else
            OnLevelLoaded?.Invoke(levelID, language);
    }
}
