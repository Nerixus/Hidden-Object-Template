using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectItem : MonoBehaviour
{
    public string levelID;
    public bool useIndexToLoad;
    public int levelIndex;
    public string language;

    public void LoadHiddenObjectLevel()
    {
        if (useIndexToLoad)
        {
            PlayerPrefs.SetInt("useIndexToLoad", 1);
            PlayerPrefs.SetInt("levelIndex", levelIndex);
        }
            
        else
        {
            PlayerPrefs.SetInt("useIndexToLoad", 0);
            PlayerPrefs.SetString("levelID", levelID);
        }
        PlayerPrefs.SetString("language", language);
        SceneManager.LoadScene(1);
    }
}
