using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ThreeLittleBerkana
{
    public class Level : MonoBehaviour
    {
        //[Header("First Setup"), Space(10)] //Uncomment if you decide to not use the editor tool
        public List<GameObject> objectsToConvert = new List<GameObject>();
        public bool alreadyHasHiddenObjects;
        public bool searchByName;
        public string hiddenObjectSubstring;

        //[Header("Setup"), Space(10)] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> levelHiddenObjects = new List<HiddenObject>();
        public bool displayLevelHiddenObjects;
        public bool allowManualChanges;
        public int numberOfObjectsToFind = 8;
        //public bool manualSetup;
        public bool shouldFindInOrder;

        //[Header("Gameplay"), Space(10)] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> hiddenObjectsToFind = new List<HiddenObject>();
        public bool selectRandomObjects;
        private Queue<HiddenObject> hiddenObjectQueue = new Queue<HiddenObject>();
        public int numberOfSimultaneousObjectsToFind = 3;
        public delegate void HandleObjectFound(HiddenObject v_foundHiddenObject, HiddenObject v_newHiddenObject);
        public static HandleObjectFound OnObjectFound;
        public delegate void HandleObjectDequeued(HiddenObject v_dequeuedObject);
        public static HandleObjectDequeued OnObjectDequeued;
        public Dictionary<string, string> levelDictionary;
        public HIDDEN_OBJECT_EXIT_MODE foundHiddenObjectFeedback;

        #region Gameplay

        public void StartLevel(LevelTemplate v_levelTemplate)
        {
            levelDictionary = LocalizationManager.GetLocalizationDictionary(GameplayManager.Instance.LANGUAGE, v_levelTemplate.localizationFile);
            foundHiddenObjectFeedback = v_levelTemplate.foundFeedback;
            SetupFoundHiddenObjectFeedback();
            SetupGameColliders();
            if (hiddenObjectsToFind != null)
            {
                if (hiddenObjectsToFind.Count == 0)
                {
                    SelectRandomHiddenObjects();
                }
            }
            else
            {
                SelectRandomHiddenObjects();
            }
            FillObjectQueue();
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
                HiddenObject newHiddenObject = hiddenObjectQueue.Dequeue();
                hiddenObjectsToFind.Add(newHiddenObject);
                OnObjectDequeued?.Invoke(newHiddenObject);
                newHiddenObject.ActivateHiddenObject();
            }
        }

        public bool IsHiddenObjectOnList(HiddenObject v_hiddenObject)
        {
            if (hiddenObjectsToFind.Contains(v_hiddenObject))
                return true;
            else
                return false;
        }

        public void ProcessHiddenObjectFound(HiddenObject v_foundHiddenObject)
        {
            //Remove found object
            hiddenObjectsToFind.Remove(v_foundHiddenObject);
            //Get new object if queue still has one
            HiddenObject newHiddenObject;
            if (hiddenObjectQueue.Count > 0)
            {
                newHiddenObject = hiddenObjectQueue.Dequeue();
                newHiddenObject.ActivateHiddenObject();
                hiddenObjectsToFind.Add(newHiddenObject);
                OnObjectFound?.Invoke(v_foundHiddenObject, newHiddenObject);
            }
            else
            {
                OnObjectFound?.Invoke(v_foundHiddenObject, null);
            }
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

        public void SetupFoundHiddenObjectFeedback()
        {
            foreach (HiddenObject HO in levelHiddenObjects)
            {
                HO.SetupHiddenObjectFoundFeedback(foundHiddenObjectFeedback);
            }
        }

        void SetupGameColliders()
        {
            foreach (HiddenObject HO in levelHiddenObjects)
            {
                HO.SetupHiddenObjectCollider();
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
                    GO.GetComponent<HiddenObject>().displayNameCode = GO.name;
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
}
