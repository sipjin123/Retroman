using UnityEngine;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Common.Utils;

/**
 * A custom SwarmItemManager for Shopping game.
 */
public class CustomSwarmItemManager : SwarmItemManager
{
    [SerializeField, ShowInInspector]
    private GameObject ActiveParent;

    [SerializeField, ShowInInspector]
    private GameObject InActiveParent;

    protected virtual void Awake()
    {
        Initialize();
    }
    
    void Update()
    {
        FrameUpdate();
    }

    /// <summary>
    /// Sets up the lists for each SwarmItem type. Also creates the parent transform for the active and inactive objects 
    /// </summary>
    public override void Initialize()
    {
        // warn the user if no prefabs were set up. There would be no need for a manager without SwarmItems
        if (itemPrefabs.Length == 0)
        {
            Debug.Log("WARNING! No Item Prefabs exists for " + gameObject.name + " -- Errors will occur.");
        }

        // make sure all the thresholds and percentages are clamped between 0 and 1.0f
        foreach (PrefabItem itemPrefab in itemPrefabs)
        {
            itemPrefab.inactiveThreshold = Mathf.Clamp01(itemPrefab.inactiveThreshold);
            itemPrefab.inactivePrunePercentage = Mathf.Clamp01(itemPrefab.inactivePrunePercentage);
        }

        // initialize the prefab item lists
        _prefabItemLists = new PrefabItemLists[itemPrefabs.Length];
        for (int i = 0; i < _prefabItemLists.Length; i++)
        {
            _prefabItemLists[i] = new PrefabItemLists();
            _prefabItemLists[i].inactivePruneTimeLeft = 0;
        }

        // create the active objects parent transform
        _activeParentTransform = ActiveParent.transform;

        // create the inactive objects parent transform;
        _inactiveParentTransform = InActiveParent.transform;
    }
}
