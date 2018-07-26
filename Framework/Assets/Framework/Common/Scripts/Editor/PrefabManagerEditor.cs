//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

using UnityEditor;
using UnityEngine;

/*
[CustomEditor(typeof(PrefabManager))]
public class PrefabManagerEditor : Editor {

	private PrefabManager prefabManager;

	void OnEnable() {
		this.prefabManager = (PrefabManager)this.target;
	}

	private int inactiveCount = 10;

	private int preloadCount = 0;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		GUILayout.BeginVertical();

		GUILayout.Space(10);

		GUILayout.Label("Populate Tool", EditorStyles.boldLabel);

		// inactive count input
		GUILayout.BeginHorizontal();
		GUILayout.Label("Inactive Count", GUILayout.Width(100));
		this.inactiveCount = EditorGUILayout.IntField(this.inactiveCount);
		GUILayout.EndHorizontal();

		if(GUILayout.Button("Populate Prune Data")) {
			PopulatePruneData();
		}

		GUILayout.Space(10);

		// preload count input
		GUILayout.BeginHorizontal();
		GUILayout.Label("Preload Count", GUILayout.Width(100));
		this.preloadCount = EditorGUILayout.IntField(this.preloadCount);
		GUILayout.EndHorizontal();

		if(GUILayout.Button("Populate Preload Data")) {
			PopulatePreloadData();
		}

		GUILayout.EndVertical();
	}
    
	private void PopulatePruneData() {
		// clamp
		if(this.inactiveCount < 0) {
			this.inactiveCount = 0;
		}

		PrefabManager.PruneData[] dataList = new PrefabManager.PruneData[prefabManager.ItemManager.itemPrefabs.Length];
		for(int i = 0; i < prefabManager.ItemManager.itemPrefabs.Length; ++i) {
			SwarmItemManager.PrefabItem current = prefabManager.ItemManager.itemPrefabs[i];
			dataList[i] = new PrefabManager.PruneData();
			dataList[i].Set(current.prefab.name, this.inactiveCount);
		}

		this.prefabManager.SetPruneDataList(dataList);
	}

	private void PopulatePreloadData() {
		// clamp
		if(this.preloadCount < 0) {
			this.preloadCount = 0;
		}

		int prefabCount = prefabManager.ItemManager.itemPrefabs.Length;
		PrefabManager.PreloadData[] dataList = new PrefabManager.PreloadData[prefabCount];
		for(int i = 0; i < prefabCount; ++i) {
			SwarmItemManager.PrefabItem current = prefabManager.ItemManager.itemPrefabs[i];
			dataList[i] = new PrefabManager.PreloadData();
			dataList[i].Set(current.prefab.name, this.preloadCount);
		}

		this.prefabManager.SetPreloadDataList(dataList);
	}
}
//*/
