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
        //MANAGERS
        public static bool _isManagerAlreadyOnScene;
        public static bool _managersFoldout;
        //LEVEL TEMPLATES
        public static bool _levelTemplatesFoldout;
        public static UnityEngine.Object levelTemplateFolder;
        public static string newLevelTemplateName;
        public GAME_TYPE templateType;
        //LEVEL PREFAB
        public static bool _createEmptyLevelPrefab;
        public static UnityEngine.Object prefabFolder;
        public static string newPrefabName;

        

        [MenuItem("Hidden Object/Hidden Object Tool")]
        public static void ShowWindow()
        {
            GetWindow<HiddenObjectTool>("Hidden Object Tool");
            if (FindObjectOfType<GameplayManager>())
            {
                _isManagerAlreadyOnScene = true;
                Debug.Log("There's a manager on scene.");
            }
            else
            {
                _isManagerAlreadyOnScene = false;
            }
        }

        private void OnGUI()
        {
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            so.Update();
            EditorStyling.DrawSplitter(10, 10);
            _managersFoldout = EditorGUILayout.Foldout(_managersFoldout, "MANAGERS");
            if (_managersFoldout)
            {
                if (_isManagerAlreadyOnScene)
                {
                    GUILayout.Label("A ---HiddenObjectManagers--- instance is already on scene.", EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label("Use the button bellow to add the HO Managers to the scene.", EditorStyles.boldLabel);
                    if (GUILayout.Button("Create Hidden Object Manager"))
                    {
                        GameObject newObject = PrefabUtility.InstantiatePrefab(Resources.Load("---HiddenObjectManagers---")) as GameObject;
                        newObject.name = "---HiddenObjectManagers---";
                        newObject.transform.position = Vector3.zero;
                        newObject.transform.rotation = Quaternion.identity;
                        _isManagerAlreadyOnScene = true;
                        EditorUtility.SetDirty(target);
                    }
                }
            }
            EditorStyling.DrawSplitter(10, 10);
            _levelTemplatesFoldout = EditorGUILayout.Foldout(_levelTemplatesFoldout, "LEVEL TEMPLATES");
            if (_levelTemplatesFoldout)
            {
                GUILayout.Label("Drag the folder where you want to create the new LevelTemplate", EditorStyles.boldLabel);
                levelTemplateFolder = EditorGUILayout.ObjectField("Target Folder:" ,levelTemplateFolder, typeof(DefaultAsset), true);
                newLevelTemplateName = EditorGUILayout.TextField("Name:", newLevelTemplateName);
                templateType = (GAME_TYPE)EditorGUILayout.EnumPopup("Template type:", templateType);
                _createEmptyLevelPrefab = EditorGUILayout.Toggle("Create Prefab Object", _createEmptyLevelPrefab);
                if (_createEmptyLevelPrefab)
                {
                    GUILayout.Label("Drag the folder where you want to create the prefab associated with this LevelTemplate", EditorStyles.boldLabel);
                    prefabFolder = EditorGUILayout.ObjectField("Target Folder:", prefabFolder, typeof(DefaultAsset), true);
                    newPrefabName = EditorGUILayout.TextField("Name:", newPrefabName);
                    if (prefabFolder != null && newPrefabName != null && levelTemplateFolder != null && newLevelTemplateName != null)
                    {
                        if (newPrefabName.Length > 0 && newLevelTemplateName.Length > 0)
                        {
                            if (GUILayout.Button("Create Level Template and Prefab"))
                            {
                                CreateLevelTemplate();
                                newLevelTemplateName = "";
                                newPrefabName = "";
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                }
                else if (levelTemplateFolder != null && newLevelTemplateName != null)
                {
                    if (newLevelTemplateName.Length > 0)
                    {
                        if (GUILayout.Button("Create Level Template"))
                        {
                            CreateLevelTemplate();
                            newLevelTemplateName = "";
                            EditorUtility.SetDirty(target);
                        }
                    }
                }
            }
            EditorStyling.DrawSplitter(10, 10);
            so.ApplyModifiedProperties();
        }
        public void CreateLevelTemplate()
        {
            LevelTemplate newLevelTemplate = CreateInstance<LevelTemplate>();
            AssetDatabase.CreateAsset(newLevelTemplate, AssetDatabase.GetAssetPath(levelTemplateFolder) + "/" + newLevelTemplateName + ".asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            if (_createEmptyLevelPrefab)
            {
                GameObject newPrefabObject = PrefabUtility.SaveAsPrefabAssetAndConnect(new GameObject(newPrefabName, typeof(Level)), AssetDatabase.GetAssetPath(prefabFolder) + "/" + newPrefabName + ".prefab", InteractionMode.UserAction, out bool prefabCreatedSuccessfully);
                if (!prefabCreatedSuccessfully)
                {
                    Debug.LogError("Prefab was not created successfully.");
                }
                else
                {
                    newLevelTemplate.levelPrefab = newPrefabObject;
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = newPrefabObject;
                }
            }
            else
            {
                Selection.activeObject = newLevelTemplate;
            }
            newLevelTemplate.gameType = templateType;
        }
    }
}
