using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace ThreeLittleBerkana
{
    [CustomEditor(typeof(Level))]
    public class Level_Editor : Editor
    {
        public SerializedProperty
            levelHiddenObjects,
            shouldFindInOrder,
            hiddenObjectsToFind,
            selectRandomObjects,
            objectsToConvert;

        public static bool _firstSetup;
        public static bool _setupArea;
        public static bool _levelDesignArea;
        public static bool _resetArea;

        public static bool _userWantsToReset;

        private void OnEnable()
        {
            levelHiddenObjects = serializedObject.FindProperty("levelHiddenObjects");
            shouldFindInOrder = serializedObject.FindProperty("shouldFindInOrder");
            hiddenObjectsToFind = serializedObject.FindProperty("hiddenObjectsToFind");
            selectRandomObjects = serializedObject.FindProperty("selectRandomObjects");
            objectsToConvert = serializedObject.FindProperty("objectsToConvert");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Level myTarget = target as Level;
            if (myTarget.levelHiddenObjects != null && myTarget.levelHiddenObjects.Count == 0)
            {
                _firstSetup = EditorGUILayout.Foldout(_firstSetup, "First Steps", true, EditorStyles.foldout);
                if (_firstSetup)
                {
                    myTarget.alreadyHasHiddenObjects = EditorGUILayout.Toggle("Already has hidden objects", myTarget.alreadyHasHiddenObjects);
                    EditorStyling.DrawSplitter(10, 10);
                    if (myTarget.alreadyHasHiddenObjects)
                    {
                        if (GUILayout.Button("Get Hidden Objects"))
                        {
                            myTarget.GetSceneHiddenObjects();
                            EditorUtility.SetDirty(myTarget);
                            _setupArea = true;
                        }
                    }
                    else
                    {
                        myTarget.searchByName = EditorGUILayout.Toggle("Search by name", myTarget.searchByName);
                        if (myTarget.searchByName)
                        {
                            GUILayout.Label("Type in the string or substring to search objects within this object.", EditorStyles.boldLabel);
                            myTarget.hiddenObjectSubstring = EditorGUILayout.TextField(myTarget.hiddenObjectSubstring);
                            if (myTarget.hiddenObjectSubstring != null)
                            {
                                if (myTarget.hiddenObjectSubstring.Length > 0)
                                {
                                    if (GUILayout.Button("Search Objects"))
                                    {
                                        myTarget.GetObjectsWithSubString();
                                    }
                                }
                            }
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(objectsToConvert, true);
                            GUI.enabled = true;
                            GUILayout.Label("*Manual changes disabled while using search by name", EditorStyles.boldLabel);
                        }
                        else
                        {
                            GUILayout.Label("Drag all objects that you wish to convert into hidden objects.", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(objectsToConvert, true);
                        }
                    }
                    if (myTarget.objectsToConvert != null && myTarget.objectsToConvert.Count > 0)
                    {
                        EditorStyling.DrawSplitterWithTitle("Options:", 10, 10);

                        GUILayout.Label("Using these buttons will set the objects as Hidden Objects if they match the type specified in the button.", EditorStyles.boldLabel);

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Set as Sprite hidden object"))
                        {
                            myTarget.FilterType<SpriteRenderer>();
                            myTarget.ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                            myTarget.ConvertToHiddenObject<SpriteRenderer, HiddenObject_Sprite>(); //We convert while validating a second time if the list wasn't cleared
                            EditorUtility.SetDirty(myTarget);
                        }
                        if (GUILayout.Button("Set as 3D hidden object"))
                        {
                            myTarget.FilterType<MeshRenderer>();
                            myTarget.ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                            myTarget.ConvertToHiddenObject<Transform, HiddenObject_3D>(); //We convert while validating a second time if the list wasn't cleared
                            EditorUtility.SetDirty(myTarget);
                        }
                        if (GUILayout.Button("Set as GUI hidden object"))
                        {
                            myTarget.FilterType<Image>();
                            myTarget.ClearExistingColliders(); //First we remove existing 2D colliders to avoid issues as well as 3D ones
                            myTarget.ConvertToHiddenObject<RectTransform, HiddenObject_UI>(); //We convert while validating a second time if the list wasn't cleared
                            EditorUtility.SetDirty(myTarget);
                        }
                        GUILayout.EndHorizontal();
                        EditorStyling.DrawSplitter(10, 10);
                        //
                    }
                }
                EditorStyling.DrawSplitter(10, 10);
            } 
            else if (myTarget.levelHiddenObjects != null && myTarget.levelHiddenObjects.Count > 0)
            {
                _setupArea = EditorGUILayout.Foldout(_setupArea, "Setup", true, EditorStyles.foldout);
                if (_setupArea)
                {
                    GUILayout.Label("Hidden Objects have been found inside this object. Current amount: " + myTarget.levelHiddenObjects.Count, EditorStyles.boldLabel);
                    myTarget.displayLevelHiddenObjects = EditorGUILayout.Toggle("Show Hidden Objects List", myTarget.displayLevelHiddenObjects);
                    if (myTarget.displayLevelHiddenObjects)
                    {
                        myTarget.allowManualChanges = EditorGUILayout.Toggle("Allow Changes", myTarget.allowManualChanges, EditorStyles.toggle);
                        if (myTarget.allowManualChanges)
                        {
                            EditorGUILayout.PropertyField(levelHiddenObjects, true);
                        }
                        else
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(levelHiddenObjects, true);
                            GUI.enabled = true;
                        }
                        
                    }
                    EditorGUILayout.Space(10);
                    myTarget.numberOfObjectsToFind = EditorGUILayout.IntField("Number of objects to find: ", myTarget.numberOfObjectsToFind);
                    if (myTarget.numberOfObjectsToFind > 0)
                    {
                        EditorGUILayout.Space(10);
                        if (GUILayout.Button("Pick " + myTarget.numberOfObjectsToFind + " random objects from the scene."))
                        {
                            myTarget.SelectRandomHiddenObjects();
                            EditorUtility.SetDirty(myTarget);
                        }
                        EditorGUILayout.Space(10);
                        EditorGUILayout.PropertyField(hiddenObjectsToFind, new GUIContent("Hidden Objects to Find"), true);
                        if (myTarget.hiddenObjectsToFind.Count > 0)
                        {
                            GUILayout.Label("Use these buttons to disable unused Hidden Objects");
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Disable"))
                            {
                                myTarget.TurnOffUnusedHiddenObjects();
                                EditorUtility.SetDirty(myTarget);
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Label("Reset all objects to their original state.");
                            if (GUILayout.Button("Reset"))
                            {
                                myTarget.ResetAllObjects();
                                EditorUtility.SetDirty(myTarget);
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("*Note that the search will happen in the children of this GameObject.", EditorStyles.label);
                    }
                }
                EditorStyling.DrawSplitter(10, 10);
                _resetArea = EditorGUILayout.Foldout(_resetArea, "Reset", true, EditorStyles.foldout);
                if (_resetArea)
                {
                    if (GUILayout.Button("Reset Level Object"))
                    {
                        _userWantsToReset = true;
                    }
                    if (_userWantsToReset)
                    {
                        GUILayout.Label("Are you sure you want to reset?", EditorStyles.label);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("YES"))
                        {
                            myTarget.ResetLevelInstanceObject();
                            _userWantsToReset = false;
                        }
                        if (GUILayout.Button("NO"))
                        {
                            _userWantsToReset = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Label("*WARNING* Doing this will reset all changes made to this level object.", EditorStyles.label);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
