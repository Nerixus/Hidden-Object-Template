using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeLittleBerkana;

public class DemoHiddenObject : MonoBehaviour
{
    public string levelID;
    public delegate void HandleLevelLoaded(string levelID);
    public static HandleLevelLoaded OnLevelLoaded;
    // Start is called before the first frame update
    void Start()
    {
        OnLevelLoaded?.Invoke(levelID);
    }
}
