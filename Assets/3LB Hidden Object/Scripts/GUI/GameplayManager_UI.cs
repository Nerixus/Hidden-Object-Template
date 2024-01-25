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
        public GameObject hiddenObjectDescriptionPrefab;
        public Transform hiddenObjectDescriptionParent;
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
                GameObject newDisplayObject = Instantiate(hiddenObjectDescriptionPrefab, hiddenObjectDescriptionParent);
                if(newDisplayObject.TryGetComponent<HiddenObject_Display>(out HiddenObject_Display displayComponent))
                {
                    displayedHiddenObjects.Add(displayComponent);
                    displayComponent.Activate(v_hiddenObject);
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