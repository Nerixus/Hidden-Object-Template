using UnityEngine;
using UnityEditor;

namespace ThreeLittleBerkana
{
    [CustomEditor(typeof(HiddenObject_Sprite)), CanEditMultipleObjects]
    public class HiddenObject_Sprite_Editor : Editor
    {
        public SerializedProperty
            displayMode,
            localizationKey,
            silhouetteSprite,
            overrideFoundFeedback,
            customFeedback,
            feedbackDuration,
            useCustomParticles,
            customParticles;

        private void OnEnable()
        {
            displayMode = serializedObject.FindProperty("displayMode");
            localizationKey = serializedObject.FindProperty("localizationKey");
            silhouetteSprite = serializedObject.FindProperty("silhouetteSprite");
            feedbackDuration = serializedObject.FindProperty("feedbackDuration");
            overrideFoundFeedback = serializedObject.FindProperty("overrideFoundFeedback");
            customFeedback = serializedObject.FindProperty("customFeedback");
            useCustomParticles = serializedObject.FindProperty("useCustomParticles");
            customParticles = serializedObject.FindProperty("customParticles");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            HiddenObject_Sprite myTarget = target as HiddenObject_Sprite;
            EditorGUILayout.PropertyField(displayMode, new GUIContent("Display Mode"), true);
            switch (myTarget.displayMode)
            {
                default:
                case DISPLAY_MODE.LOCALIZATION_NAME:
                    EditorGUILayout.PropertyField(localizationKey, new GUIContent("Localization Key"), true);
                    break;
                case DISPLAY_MODE.SILHOUETTE:
                    EditorGUILayout.PropertyField(silhouetteSprite, new GUIContent("Silhouette Sprite"), true);
                    break;
            }
            EditorGUILayout.PropertyField(feedbackDuration, new GUIContent("Feedback duration"), true);
            EditorGUILayout.PropertyField(overrideFoundFeedback, new GUIContent("Override found feedback"), true);
            if (myTarget.overrideFoundFeedback)
            {
                EditorGUILayout.PropertyField(customFeedback, new GUIContent("Feedback Options"), true);
            }
            EditorGUILayout.PropertyField(useCustomParticles, new GUIContent("Override found particles"), true);
            if (myTarget.useCustomParticles)
            {
                EditorGUILayout.PropertyField(customParticles, new GUIContent("Found particles"), true);
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(myTarget);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
