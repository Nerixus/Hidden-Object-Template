using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThreeLittleBerkana
{
    public class EditorStyling : Editor
    {
		static readonly Color _splitterdark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
		static readonly Color _splitterlight = new Color(0.6f, 0.6f, 0.6f, 1.333f);
		public static Color Splitter { get { return EditorGUIUtility.isProSkin ? _splitterdark : _splitterlight; } }
		static public void DrawSplitterWithTitle(string title, float spaceBefore, float spaceAfter)
		{
			DrawSplitter(spaceBefore, spaceAfter);
			EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
			EditorGUILayout.Space(10);
		}

		/// <summary>
		/// Draw a separator line
		/// </summary>
		static public void DrawSplitLine() // Draw a Splitter Line with no spaces
		{
			// Helper to draw a separator line

			var rect = GUILayoutUtility.GetRect(1f, 1f);

			rect.xMin = 0f;
			rect.width += 4f;

			if (Event.current.type != EventType.Repaint)
				return;

			EditorGUI.DrawRect(rect, Splitter);
		}

		static public void DrawSplitter(float spaceBefore, float spaceAfter) // Draw a Splitter Line with space before and after 
		{
			EditorGUILayout.Space(spaceBefore);
			DrawSplitLine();
			EditorGUILayout.Space(spaceAfter);
		}
	}
}
