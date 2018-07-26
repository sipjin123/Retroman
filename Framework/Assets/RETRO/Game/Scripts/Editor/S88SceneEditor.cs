using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using Framework;

namespace Synergy88 {
	
	[CustomEditor(typeof(Scene))]
	public class SceneEditor : S88Editor  {
		
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