using UnityEngine;
using UnityEditor;
//using UnityEngine.UI;
//using UnityEditorInternal;
using System.Collections.Generic;
//using System.Collections;
using System.IO;
using UnityEngine.UI;
//using System.Runtime.Serialization.Formatters.Binary;
//using System;

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
        //LOCALIZATION
        public static bool _localizationFoldout;
        public static bool _newDictionary;
        public static bool _editDictionary;
        public GameObject localizationLevel;
        public SerializedProperty levelToLocalize;
        public string[] languages;
        public SerializedProperty languagesToAdd;
        public TextAsset localizationFile;
        public SerializedProperty localizationProperty;

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
            if(languagesToAdd == null)
                languagesToAdd = so.FindProperty("languages");
            if (levelToLocalize == null)
                levelToLocalize = so.FindProperty("localizationLevel");
            if (localizationProperty == null)
                localizationProperty = so.FindProperty("localizationFile");
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
            _localizationFoldout = EditorGUILayout.Foldout(_localizationFoldout, "LOCALIZATION");
            if (_localizationFoldout)
            {
                //localizationLevel = EditorGUILayout.ObjectField("Level to Localize", localizationLevel, typeof(GameObject), true) as GameObject;
                EditorGUILayout.PropertyField(levelToLocalize, new GUIContent("Level to localize"), true);
                _newDictionary = EditorGUILayout.Foldout(_newDictionary, "New Dictionary");
                if (_newDictionary)
                {
                    GUILayout.Label("Type the language headers you'll use for localization.", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(languagesToAdd, new GUIContent("Languages to add"), true);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create Localization Dictionary"))
                    {
                        if (localizationLevel != null)
                        {
                            var path = EditorUtility.SaveFilePanel("Save new csv", Application.dataPath, localizationLevel.gameObject.name, "csv");
                            if (path.Length != 0)
                            {
                                CreateCsvFile(localizationLevel, path);
                            }
                        }
                        
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorStyling.DrawSplitter(10, 10);
            so.ApplyModifiedProperties();
        }
        public void CreateLevelTemplate()
        {
            LevelTemplate newLevelTemplate = CreateInstance<LevelTemplate>();
            AssetDatabase.CreateAsset(newLevelTemplate, AssetDatabase.GetAssetPath(levelTemplateFolder) + "/" + newLevelTemplateName + ".asset");
            newLevelTemplate.gameType = templateType;
            if (_createEmptyLevelPrefab)
            {
                GameObject newPrefabObject;
                if (templateType == GAME_TYPE.TWO_D_UI_WIP)
                {
                    newPrefabObject = PrefabUtility.SaveAsPrefabAssetAndConnect(new GameObject(newPrefabName, typeof(Canvas)), AssetDatabase.GetAssetPath(prefabFolder) + "/" + newPrefabName + ".prefab", InteractionMode.UserAction, out bool prefabCreatedSuccessfully);
                    CanvasScaler canvasScaler = newPrefabObject.AddComponent<CanvasScaler>();
                    GraphicRaycaster graphicRaycaster = newPrefabObject.AddComponent<GraphicRaycaster>();
                    Canvas canvas = newPrefabObject.GetComponent<Canvas>();
                    newPrefabObject.AddComponent<Level>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    if (!prefabCreatedSuccessfully)
                    {
                        Debug.LogError("Prefab was not created successfully.");
                    }
                    else
                    {
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = newPrefabObject;
                    }
                }
                else
                {
                    newPrefabObject = PrefabUtility.SaveAsPrefabAssetAndConnect(new GameObject(newPrefabName, typeof(Level)), AssetDatabase.GetAssetPath(prefabFolder) + "/" + newPrefabName + ".prefab", InteractionMode.UserAction, out bool prefabCreatedSuccessfully);
                    if (!prefabCreatedSuccessfully)
                    {
                        Debug.LogError("Prefab was not created successfully.");
                    }
                    else
                    {
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = newPrefabObject;
                    }
                }
            }
            else
            {
                Selection.activeObject = newLevelTemplate;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
        }

        public void CreateCsvFile(GameObject v_levelObject, string v_path)
        {
            //Create Headers
            string csvValues = "\"ObjectKey\"";
            string csvLine = "";
            for (int i = 0; i < languages.Length; i++)
            {
                csvValues += ",\"" + languages[i] + "\"";
                csvLine += ",\"\"";
            }
            csvValues += "\n";
            //Create object list with Key
            List<HiddenObject> levelHiddenObjects = new List<HiddenObject>();
            levelHiddenObjects.AddRange(v_levelObject.GetComponentsInChildren<HiddenObject>(true));
            for (int i = 0; i < levelHiddenObjects.Count; i++)
            {
                csvValues += "\"" + levelHiddenObjects[i].gameObject.name + "\"" + csvLine;
                if (i != levelHiddenObjects.Count - 1)
                {
                    csvValues += "\n";
                }
            }
            //Create text file
            using StreamWriter file = new StreamWriter(v_path);
            file.Write(csvValues);
            Debug.Log("Dictionary Created");
            AssetDatabase.Refresh();
        }
    }
}
