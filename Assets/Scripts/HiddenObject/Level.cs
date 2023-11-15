using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ThreeLittleBerkana
{
    [ExecuteAlways]
    public class Level : MonoBehaviour
    {
        public List<HiddenObject> sceneHiddenObjects = new List<HiddenObject>();
        public int numberOfObjectsToFind;
        public bool manualSetup;
        public SETUP_TYPE setupType;
        public bool shouldFindInOrder;
        private Queue<HiddenObject> hiddenObjectQueue; //In case we want objects to be found in a specific order.
        public bool selectRandomObjects;
        public List<HiddenObject> hiddenObjectsToFind = new List<HiddenObject>();

        #region Utilities
        public void GetSceneHiddenObjects()
        {
            sceneHiddenObjects.Clear();
            sceneHiddenObjects.AddRange(GetComponentsInChildren<HiddenObject>(true));
            //Debug.Log("Found " + sceneHiddenObjects.Count + " objects");
        }
        public void SelectRandomHiddenObjects()
        {
            GetSceneHiddenObjects();
            hiddenObjectsToFind.Clear();
            for (int i = 0; i < numberOfObjectsToFind; i++)
            {
                int randInt = Random.Range(0, sceneHiddenObjects.Count);
                hiddenObjectsToFind.Add(sceneHiddenObjects[randInt]);
                sceneHiddenObjects.Remove(sceneHiddenObjects[randInt]);
            }
            GetSceneHiddenObjects();
        }
        public void TurnOffUnusedHiddenObjects()
        {
            GetSceneHiddenObjects();
            foreach (HiddenObject HO in sceneHiddenObjects)
            {
                if (hiddenObjectsToFind.Contains(HO))
                    HO.gameObject.SetActive(true);
                else
                    HO.gameObject.SetActive(false);
            }
        }
        public void TurnOffUnusedHiddenObjectColliders()
        {
            GetSceneHiddenObjects();
            foreach (HiddenObject HO in sceneHiddenObjects)
            {
                if (hiddenObjectsToFind.Contains(HO))
                {
                    if(HO.TryGetComponent<Collider>(out Collider col))
                        col.enabled = true;
                    if (HO.TryGetComponent<Collider2D>(out Collider2D col2d))
                        col2d.enabled = true;
                }
                else
                {
                    if (HO.TryGetComponent<Collider>(out Collider col))
                        col.enabled = false;
                    if (HO.TryGetComponent<Collider2D>(out Collider2D col2d))
                        col2d.enabled = false;
                }
            }
        }
        public void ResetAllObjects()
        {
            foreach (HiddenObject HO in sceneHiddenObjects)
            {
                HO.gameObject.SetActive(true);
                if (HO.TryGetComponent<Collider>(out Collider col))
                    col.enabled = true;
                if (HO.TryGetComponent<Collider2D>(out Collider2D col2d))
                    col2d.enabled = true;
            }
        }

        #endregion

        public bool IsHiddenObjectOnList(HiddenObject v_hiddenObject)
        {
            if (hiddenObjectsToFind.Contains(v_hiddenObject))
                return true;
            else
                return false;
        }

        public void ProcessHiddenObjectFound(HiddenObject hiddenObject)
        {

        }
    }

    public enum SETUP_TYPE
    {
        RANDOM,
        ORDERED
    }
}
