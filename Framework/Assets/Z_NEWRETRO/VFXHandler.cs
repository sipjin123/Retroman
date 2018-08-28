using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Framework;
using Common;
using Common.Utils;

public class VFXHandler : SerializedMonoBehaviour
{
    public enum VFXList
    {
        JumpVFX,
        NewVFX,
        None,
        BumpVFX
    }


    [SerializeField]
    PrefabManager prefManager;

    [SerializeField]
    Dictionary<VFXList, GameObject> VFXDict; 

	void Awake()
    {

        Factory.Register<VFXHandler>(this);
	}
    private void OnDestroy()
    {
        Factory.Clean< VFXHandler>();
    }
    public void RequestVFX(Vector3 thisLocation,VFXList vfxToRequest)
    {
        GameObject obj =  prefManager.Request(VFXDict[vfxToRequest].name);
        obj.transform.position = thisLocation;

    }


	void Update ()
    {
		
	}
}
