using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeLittleBerkana
{
    public class GameplayManager_UI : MonoBehaviour
    {
        public GameObject hiddenObjectDescriptionPrefab;
        public List<HiddenObject_Display> displayedHiddenObjects = new List<HiddenObject_Display>();

        public GameObject winScreen;

        private void OnEnable()
        {
            GameplayManager.OnGameVictory += ShowWinScreen;
        }

        private void OnDisable()
        {
            GameplayManager.OnGameVictory -= ShowWinScreen;
        }
        public void DisplayHiddenObjectName(HiddenObject v_hiddenObject)
        {
            
        }

        void ShowWinScreen()
        {
            winScreen.SetActive(true);
        }
    }
}
