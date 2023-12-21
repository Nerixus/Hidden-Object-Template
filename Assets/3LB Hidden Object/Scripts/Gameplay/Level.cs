using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ThreeLittleBerkana
{
    public class Level : MonoBehaviour
    {
        //[Header("First Setup")] //Uncomment if you decide to not use the editor tool
        public List<GameObject> objectsToConvert = new List<GameObject>();
        public bool alreadyHasHiddenObjects;
        public bool searchByName;
        public string hiddenObjectSubstring;

        //[Header("Setup")] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> levelHiddenObjects = new List<HiddenObject>();
        public bool displayLevelHiddenObjects;
        public bool allowManualChanges;
        public int numberOfObjectsToFind = 8;
        public bool manualSetup;
        public SETUP_TYPE setupType;
        public bool shouldFindInOrder;

        //[Header("Gameplay")] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> hiddenObjectsToFind = new List<HiddenObject>();
        public bool selectRandomObjects;
        private Queue<HiddenObject> hiddenObjectQueue = new Queue<HiddenObject>();
        public int numberOfSimultaneousObjectsToFind = 3;


        #region Gameplay

        private void Start()
        {
            if (hiddenObjectsToFind != null)
            {
                if (hiddenObjectsToFind.Count > 0)
                {
                    FillObjectQueue();
                }
            }
            else
            {
                SelectRandomHiddenObjects();
            }
            SetupGame();
        }

        private void FillObjectQueue()
        {
            foreach (HiddenObject HO in hiddenObjectsToFind)
            {
                hiddenObjectQueue.Enqueue(HO);
            }
            hiddenObjectsToFind.Clear();
        }

        private void SetupGame()
        {
            for (int i = 0; i < numberOfSimultaneousObjectsToFind; i++)
            {
                hiddenObjectsToFind.Add(hiddenObjectQueue.Dequeue());
            }
        }

        public bool IsHiddenObjectOnList(HiddenObject v_hiddenObject)
        {
            if (hiddenObjectsToFind.Contains(v_hiddenObject))//ganti
                return true;
            else
                return false;
        }

        public void ProcessHiddenObjectFound(HiddenObject hiddenObject)
        {
            hiddenObjectsToFind.Remove(hiddenObject);
            if (hiddenObjectQueue.Count > 0)
                hiddenObjectsToFind.Add(hiddenObjectQueue.Dequeue());
        }

        public bool IsHiddenObjectListEmpty()
        {
            if (hiddenObjectsToFind.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Setup

        #region Initial Setup
        public void ClearExistingColliders()
        {
            foreach (GameObject GO in objectsToConvert)
            {
                Collider2D[] existingColliders2D = GO.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D C2D in existingColliders2D)
                {
                    DestroyImmediate(C2D);
                }
                Collider[] existingColliders3D = GO.GetComponentsInChildren<Collider>();
                foreach (Collider CO in existingColliders3D)
                {
                    DestroyImmediate(CO);
                }
                EditorUtility.SetDirty(GO);
            }
        }

        public void FilterType<T>() where T : Component
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            foreach (GameObject GO in objectsToConvert)
            {
                var v_component = GO.GetComponent<T>();
                if (v_component == null)
                {
                    objectsToRemove.Add(GO);
                }
                EditorUtility.SetDirty(GO);
            }
            foreach (GameObject GO in objectsToRemove)
            {
                objectsToConvert.Remove(GO);
                EditorUtility.SetDirty(GO);
            }
            objectsToRemove.Clear();
        }

        public void ConvertToHiddenObject<T, U>() where T : Component where U : Component
        {
            int errorCount = 0;
            foreach (GameObject GO in objectsToConvert)
            {
                var v_component = GO.GetComponent<T>();
                if (v_component != null)
                {
                    if (GO.GetComponent<U>() == null)
                        GO.AddComponent<U>();
                    levelHiddenObjects.Add(GO.GetComponent<HiddenObject>());
                }
                else
                {
                    errorCount++;
                }
                EditorUtility.SetDirty(GO);
            }
            if (errorCount > 0)
            {
                Debug.LogWarning("Some objects were excluded from the process since they are the wrong type specified for conversion. Number of objects excluded: " + errorCount);
            }
            objectsToConvert.Clear();
        }

        public void GetObjectsWithSubString()
        {
            objectsToConvert.Clear();
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Contains(hiddenObjectSubstring))
                    objectsToConvert.Add(child.gameObject);
            }
        }
        #endregion

        #region SecondarySetup
        public void GetSceneHiddenObjects()
        {
            levelHiddenObjects.Clear();
            levelHiddenObjects.AddRange(GetComponentsInChildren<HiddenObject>(true));
            if (levelHiddenObjects.Count == 0)
                Debug.LogWarning("No hidden objects were found inside this level object");
        }

        public void GetSceneHiddenObjects(string v_name)
        {
            levelHiddenObjects.Clear();
            //sceneHiddenObjects.AddRange()
        }
        public void SelectRandomHiddenObjects()
        {
            GetSceneHiddenObjects();
            hiddenObjectsToFind.Clear();
            if (levelHiddenObjects.Count < numberOfObjectsToFind)
            {
                numberOfObjectsToFind = levelHiddenObjects.Count;
                Debug.LogWarning("The number of \"Objects To Find\" has been adjusted because you don't have enough Hidden Objects in \"Scene Hidden Objects\".");
            }
            for (int i = 0; i < numberOfObjectsToFind; i++)
            {
                int randInt = Random.Range(0, levelHiddenObjects.Count);
                hiddenObjectsToFind.Add(levelHiddenObjects[randInt]);
                levelHiddenObjects.Remove(levelHiddenObjects[randInt]);
            }
            GetSceneHiddenObjects();
        }
        public void TurnOffUnusedHiddenObjects()
        {
            GetSceneHiddenObjects();
            foreach (HiddenObject HO in levelHiddenObjects)
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
            foreach (HiddenObject HO in levelHiddenObjects)
            {
                if (hiddenObjectsToFind.Contains(HO))
                {
                    if (HO.TryGetComponent<Collider>(out Collider col))
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
            foreach (HiddenObject HO in levelHiddenObjects)
            {
                HO.gameObject.SetActive(true);
                if (HO.TryGetComponent<Collider>(out Collider col))
                    col.enabled = true;
                if (HO.TryGetComponent<Collider2D>(out Collider2D col2d))
                    col2d.enabled = true;
            }
        }

        public void ResetLevelInstanceObject()
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.TryGetComponent<HiddenObject>(out HiddenObject ho))
                    DestroyImmediate(ho);
                if (child.TryGetComponent<Collider>(out Collider col))
                    DestroyImmediate(col);
                if (child.TryGetComponent<Collider2D>(out Collider2D col2d))
                    DestroyImmediate(col2d);
            }
            objectsToConvert.Clear();
            levelHiddenObjects.Clear();
            hiddenObjectsToFind.Clear();
        }
        #endregion

        #endregion
    }

    public enum SETUP_TYPE
    {
        RANDOM,
        ORDERED
    }
}
