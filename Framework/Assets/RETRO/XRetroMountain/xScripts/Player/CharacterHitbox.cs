using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour {
	
	void OnTriggerEnter(Collider hit)
	{
		if(LayerMask.LayerToName( hit.gameObject.layer) == "GroundOnly")
		{
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal());
		}
	}
}
