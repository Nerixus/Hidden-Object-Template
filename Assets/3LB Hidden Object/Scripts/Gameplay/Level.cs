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
        public List<GameObject> objectsToSetUp = new List<GameObject>();
        public bool containsHiddenObjects;
        public bool searchByName;
        public string hiddenObjectSubstring;
        public bool shouldPreserveColliders;

        //[Header("Setup"), Space(10)] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> levelHiddenObjects = new List<HiddenObject>();
        public bool displayLevelHiddenObjects;
        public bool allowHiddenObjectListChanges;
        public int numberOfObjectsToFind = 8;

        //[Header("Gameplay"), Space(10)] //Uncomment if you decide to not use the editor tool
        public List<HiddenObject> hiddenObjectsToFind = new List<HiddenObject>();
        private Queue<HiddenObject> hiddenObjectQueue = new Queue<HiddenObject>();
        public int numberOfSimultaneousObjectsToFind = 3;
        public Dictionary<string, string> levelDictionary;

        public delegate void HandleObjectFound(HiddenObject v_foundHiddenObject, HiddenObject v_newHiddenObject);
        public static HandleObjectFound OnObjectFound;
        public delegate void HandleObjectDequeued(HiddenObject v_dequeuedObject);
        public static HandleObjectDequeued OnObjectDequeued;
        #region Gameplay

        public void StartLevel(LevelTemplate v_levelTemplate)
        {
            levelDictionary = LocalizationManager.GetLocalizationDictionary(GameplayManager.Instance.LANGUAGE, v_levelTemplate.localizationFile);
            SetupFoundHiddenObjectFeedbacks(v_levelTemplate);
            SetupGameColliders();
            numberOfSimultaneousObjectsToFind = v_levelTemplate.simultaneousObjectsToFind;
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
                if (hiddenObjectQueue.Count > 0)
                {
                    HiddenObject newHiddenObject = hiddenObjectQueue.Dequeue();
                    hiddenObjectsToFind.Add(newHiddenObject);
                    OnObjectDequeued?.Invoke(newHiddenObject);
                    newHiddenObject.ActivateHiddenObject();
                }
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

        public void SetupFoundHiddenObjectFeedbacks(LevelTemplate v_levelTemplate)
        {
            foreach (HiddenObject HO in levelHiddenObjects)
            {
                HO.SetupHiddenObjectFoundFeedback(v_levelTemplate.foundFeedback);
                HO.SetupHiddenObjectFoundParticles(v_levelTemplate.foundParticles);
            }
        }

        private void SetupGameColliders()
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
            foreach (GameObject GO in objectsToSetUp)
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
            foreach (GameObject GO in objectsToSetUp)
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
                objectsToSetUp.Remove(GO);
                EditorUtility.SetDirty(GO);
            }
            objectsToRemove.Clear();
        }

        public void SetAsHiddenObject<T, U>() where T : Component where U : Component
        {
            int errorCount = 0;
            foreach (GameObject GO in objectsToSetUp)
            {
                var v_component = GO.GetComponent<T>();
                if (v_component != null)
                {
                    if (GO.GetComponent<U>() == null)
                        GO.AddComponent<U>();
                    levelHiddenObjects.Add(GO.GetComponent<HiddenObject>());
                    GO.GetComponent<HiddenObject>().localizationKey = GO.name;
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
            objectsToSetUp.Clear();
        }

        public void GetObjectsWithSubString()
        {
            objectsToSetUp.Clear();
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Contains(hiddenObjectSubstring))
                    objectsToSetUp.Add(child.gameObject);
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

        public void TurnOnAllObjects()
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
            objectsToSetUp.Clear();
            levelHiddenObjects.Clear();
            hiddenObjectsToFind.Clear();
        }

        
        #endregion

        #endregion
    }
}
