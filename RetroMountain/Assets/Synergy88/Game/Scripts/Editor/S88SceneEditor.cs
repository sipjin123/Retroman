using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Synergy88 {
	
	[CustomEditor(typeof(S88Scene))]
	public class S88SceneEditor : S88Editor  {
		
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			DrawDefaultInspector();

			DrawButton("Load Scene", () => {
			});

			DrawButton("Unload Scene", () => {
			});
		}
	}

}