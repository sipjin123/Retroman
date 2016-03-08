using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Synergy88 {

	public class S88Editor : Editor  {
		
		protected static bool DrawButton(string title, string tooltip, float width) {
			return GUILayout.Button(new GUIContent(title, tooltip));
		}

		protected static void DrawButton(string title, Action visitor) {
			EditorGUILayout.BeginHorizontal();
			if (DrawButton(title, string.Empty, 40f)) {
				visitor();
			}
			EditorGUILayout.EndHorizontal();
		}

	}

}