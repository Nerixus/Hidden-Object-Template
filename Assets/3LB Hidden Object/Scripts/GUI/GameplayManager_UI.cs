using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeLittleBerkana
{
    public class GameplayManager_UI : StaticInstance<GameplayManager_UI>
    {
        [Header("Panels")]
        public GameObject victoryScreen;
        public GameObject hudPanel;

        [Header("Objects to find")]
        public GameObject hiddenObjectTextDisplayPrefab;
        public Transform hiddenObjectTextDisplayParent;
        public GameObject hiddenObjectSilhouetteDisplayPrefab;
        public Transform hiddenObjectSilhouetteDisplayParent;
        public List<HiddenObject_Display> displayedHiddenObjects = new List<HiddenObject_Display>();

        

        private void OnEnable()
        {
            GameplayManager.OnGameVictory += DisplayVictoryScreen;
            Level.OnObjectFound += ProcessHiddenObjectFound;
            Level.OnObjectDequeued += CreateDisplayObjectList;
            HiddenObject_Display.OnDisplayOff += RemoveHiddenObjectDisplayFromList;
        }

        private void OnDisable()
        {
            GameplayManager.OnGameVictory -= DisplayVictoryScreen;
            Level.OnObjectFound -= ProcessHiddenObjectFound;
            Level.OnObjectDequeued -= CreateDisplayObjectList;
            HiddenObject_Display.OnDisplayOff -= RemoveHiddenObjectDisplayFromList;
        }
        private void CreateDisplayObjectList(HiddenObject v_hiddenObject)
        {
            if (displayedHiddenObjects.Count < GameplayManager.Instance.CurrentLoadedLevel.numberOfSimultaneousObjectsToFind)
            {
                switch (GameplayManager.Instance.DisplayMode)
                {
                    default:
                    case DISPLAY_MODE.LOCALIZATION_NAME:
                        GameObject newTextDisplayObject = Instantiate(hiddenObjectTextDisplayPrefab, hiddenObjectTextDisplayParent);
                        if (newTextDisplayObject.TryGetComponent<HiddenObject_Display>(out HiddenObject_Display textDisplayComponent))
                        {
                            displayedHiddenObjects.Add(textDisplayComponent);
                            textDisplayComponent.Activate(v_hiddenObject);
                        }
                        break;
                    case DISPLAY_MODE.SILHOUETTE:
                        GameObject newSilhoutteDisplayObject = Instantiate(hiddenObjectSilhouetteDisplayPrefab, hiddenObjectSilhouetteDisplayParent);
                        if (newSilhoutteDisplayObject.TryGetComponent<HiddenObject_Display>(out HiddenObject_Display silhoutteDisplayComponent))
                        {
                            displayedHiddenObjects.Add(silhoutteDisplayComponent);
                            silhoutteDisplayComponent.Activate(v_hiddenObject);
                        }
                        break;
                }
                
            }
        }

        private void ProcessHiddenObjectFound(HiddenObject v_foundHiddenObject, HiddenObject v_newHiddenObject)
        {
            if (v_newHiddenObject != null) //if there's a new dequeued obj
            {
                foreach (HiddenObject_Display HOD in displayedHiddenObjects)
                {
                    if (HOD.CompareHiddenObjectComponent(v_foundHiddenObject))
                        HOD.SwitchDisplayedObject(v_newHiddenObject);
                }
            }
            else //if there's no more objects to dequeue
            {
                foreach (HiddenObject_Display HOD in displayedHiddenObjects)
                {
                    if (HOD.CompareHiddenObjectComponent(v_foundHiddenObject))
                        HOD.TurnOff();
                }
            }
            
        }
        private void DisplayVictoryScreen()
        {
            victoryScreen.SetActive(true);
        }

        private void RemoveHiddenObjectDisplayFromList(HiddenObject_Display v_hiddenObjectDisplay)
        {
            if (displayedHiddenObjects.Contains(v_hiddenObjectDisplay))
                displayedHiddenObjects.Remove(v_hiddenObjectDisplay);
            Destroy(v_hiddenObjectDisplay.gameObject);
        }
    }
}