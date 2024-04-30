using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoHiddenObject : MonoBehaviour
{
    public string levelID;
    public bool useIndexToLoad;
    public int levelIndex;
    public string language;

    void Start()
    {
        if (PlayerPrefs.HasKey("useIndexToLoad"))
        {
            if (PlayerPrefs.GetInt("useIndexToLoad") == 1)
            {
                ThreeLittleBerkana.GameplayManager.Instance.LoadHiddenObjectLevel(PlayerPrefs.GetInt("levelIndex"), PlayerPrefs.GetString("language"));
            }
            else
            {
                ThreeLittleBerkana.GameplayManager.Instance.LoadHiddenObjectLevel(PlayerPrefs.GetString("levelID"), PlayerPrefs.GetString("language"));
            }
        }
        else
        {
            if (useIndexToLoad)
                ThreeLittleBerkana.GameplayManager.Instance.LoadHiddenObjectLevel(levelIndex, language);
            else
                ThreeLittleBerkana.GameplayManager.Instance.LoadHiddenObjectLevel(levelID, language);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
