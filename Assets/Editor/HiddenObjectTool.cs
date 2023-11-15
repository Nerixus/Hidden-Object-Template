using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace ThreeLittleBerkana
{
    public class HiddenObjectTool : EditorWindow
    {
        //public GameObject[] objectsToConvert;
        public List<GameObject> objectsToConvert;
        SerializedObject displayHiddenObject;
        int hiddenObjsCount = 0;
        public GameObject hoToEdit;
        public TextAsset csvFile;


        [MenuItem("3LB/Hidden Object Tool")]
        public static void ShowWindow()
        {
            GetWindow<HiddenObjectTool>("Hidden Object Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Drag all objects that you wish to convert into hidden objects.", EditorStyles.boldLabel);

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty hiddenObjectsProperty = so.FindProperty("objectsToConvert");

            EditorGUILayout.PropertyField(hiddenObjectsProperty, true);
            so.ApplyModifiedProperties();

            if (objectsToConvert != null && objectsToConvert.Count > 0)
            {
                GUILayout.Label("Use these to filter the object type you want to convert to Hidden Objects.", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Filter Objects: Sprite Renderer"))
                {
                    FilterType<SpriteRenderer>(); //We clear the objects that missmatch the type we need.
                }
                if (GUILayout.Button("Filter Objects: Mesh Renderer"))
                {
                    FilterType<MeshRenderer>(); //We clear the objects that missmatch the type we need.
                }
                if (GUILayout.Button("Filter Objects: Image (UI)"))
                {
                    FilterType<Image>(); //We clear the objects that missmatch the type we need.
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("Use these to convert to Hidden Objects.", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Convert to Sprite hidden object"))
                {
                    ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                    ConvertToHiddenObject<SpriteRenderer, HiddenObject_Sprite>(); //We convert while validating a second time if the list wasn't cleared
                }
                if (GUILayout.Button("Convert to 3D hidden object"))
                {
                    ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                    ConvertToHiddenObject<MeshRenderer, HiddenObject_3D>(); //We convert while validating a second time if the list wasn't cleared
                }
                if (GUILayout.Button("Convert to GUI hidden object"))
                {
                    ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                    ConvertToHiddenObject<Image, HiddenObject_UI>(); //We convert while validating a second time if the list wasn't cleared
                }
                GUILayout.EndHorizontal();
            }
        }

        void ClearExistingColliders()
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
            }
        }

        void FilterType<T>() where T : Component
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            foreach (GameObject GO in objectsToConvert)
            {
                var v_component = GO.GetComponent<T>();
                if (v_component == null)
                {
                    objectsToRemove.Add(GO);
                }
            }
            foreach (GameObject GO in objectsToRemove)
            {
                objectsToConvert.Remove(GO);
            }
            objectsToRemove.Clear();
        }

        void ConvertToHiddenObject<T, U>() where T : Component where U : Component
        {
            int errorCount = 0;
            foreach (GameObject GO in objectsToConvert)
            {
                var v_component = GO.GetComponent<T>();
                if (v_component != null)
                {
                    if (GO.GetComponent<U>() == null)
                        GO.AddComponent<U>();
                }
                else
                {
                    errorCount++;
                }
            }
            if (errorCount > 0)
            {
                Debug.LogWarning("Some objects were excluded from the process since they are the wrong type specified for conversion. Number of objects excluded: " + errorCount);
            }
        }
    }

}
