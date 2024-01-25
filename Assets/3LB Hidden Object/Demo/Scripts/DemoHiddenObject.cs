using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeLittleBerkana;

public class DemoHiddenObject : MonoBehaviour
{
    public string levelID;
    public string language;

    public delegate void HandleLevelLoaded(string v_levelID, string v_language);
    public static HandleLevelLoaded OnLevelLoaded;
    // Start is called before the first frame update
    void Start()
    {
        OnLevelLoaded?.Invoke(levelID, language);
    }
}
