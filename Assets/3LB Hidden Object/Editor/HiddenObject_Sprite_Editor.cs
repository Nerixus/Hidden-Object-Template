using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThreeLittleBerkana
{
    [CustomEditor(typeof(HiddenObject_Sprite))]
    [CanEditMultipleObjects]
    public class HiddenObject_Sprite_Editor : Editor
    {
		public SerializedProperty
		useCustomParticles,
		foundParticles;

		private void OnEnable()
        {
            useCustomParticles = serializedObject.FindProperty("useCustomParticles");
            foundParticles = serializedObject.FindProperty("foundParticles");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            HiddenObject_Sprite myTarget = target as HiddenObject_Sprite;
            myTarget.useCustomParticles = EditorGUILayout.Toggle(new GUIContent("Use Custom Particles"), myTarget.useCustomParticles);
            if (myTarget.useCustomParticles)
            {
                EditorGUILayout.PropertyField(foundParticles, new GUIContent("Found Particles"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
