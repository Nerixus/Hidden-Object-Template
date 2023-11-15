using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThreeLittleBerkana
{
    [CustomEditor(typeof(Level))]
    public class Level_Editor : Editor
    {
        public SerializedProperty
            sceneHiddenObjects,
            shouldFindInOrder,
            hiddenObjectsToFind,
            selectRandomObjects;

        public static bool _setupArea;
        //bool manualSetup;
        //int numberOfObjectsToFind;

        private void OnEnable()
        {
            sceneHiddenObjects = serializedObject.FindProperty("sceneHiddenObjects");
            shouldFindInOrder = serializedObject.FindProperty("shouldFindInOrder");
            hiddenObjectsToFind = serializedObject.FindProperty("hiddenObjectsToFind");
            selectRandomObjects = serializedObject.FindProperty("selectRandomObjects");
        }
        public override void OnInspectorGUI()
        {
            Level myTarget = target as Level;

            _setupArea = EditorGUILayout.Foldout(_setupArea, "Setup", true, EditorStyles.foldout);
            if (_setupArea)
            {
                if (GUILayout.Button("Validate Hidden Objects In Scene."))
                {
                    myTarget.GetSceneHiddenObjects();
                }
                if (myTarget.sceneHiddenObjects != null && myTarget.sceneHiddenObjects.Count > 0)
                {
                    GUILayout.Label("Hidden Objects have been found in scene. Current amount: " + myTarget.sceneHiddenObjects.Count, EditorStyles.boldLabel);
                    EditorGUILayout.Space(10);
                    myTarget.numberOfObjectsToFind = EditorGUILayout.IntField("Number of objects to find: ", myTarget.numberOfObjectsToFind);
                    if (myTarget.numberOfObjectsToFind > 0)
                    {

                        myTarget.manualSetup = EditorGUILayout.Toggle("Manual Setup", myTarget.manualSetup);
                        if (myTarget.manualSetup)
                        {
                            myTarget.setupType = (SETUP_TYPE)EditorGUILayout.EnumPopup("Setup Type", myTarget.setupType);
                            switch (myTarget.setupType)
                            {
                                default:
                                case SETUP_TYPE.RANDOM:
                                    if (GUILayout.Button("Pick " + myTarget.numberOfObjectsToFind + " random objects from the scene"))
                                    {
                                        myTarget.SelectRandomHiddenObjects();
                                    }
                                    break;
                                case SETUP_TYPE.ORDERED:
                                    GUILayout.Label("Drag the objects you wish to setup.");
                                    break;
                            }
                            EditorGUILayout.PropertyField(hiddenObjectsToFind, new GUIContent("Hidden Objects to Find"), true);
                            if (myTarget.hiddenObjectsToFind.Count > 0)
                            {
                                GUILayout.Label("Use these buttons to disable unused Hidden Objects");
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button("Game Object"))
                                {
                                    myTarget.TurnOffUnusedHiddenObjects();
                                }
                                if (GUILayout.Button("Colliders"))
                                {
                                    myTarget.TurnOffUnusedHiddenObjectColliders();
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.Label("Reset all objects to their original state.");
                                if (GUILayout.Button("Reset"))
                                {
                                    myTarget.ResetAllObjects();
                                }
                            }
                        }
                    }
                }
            }
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
