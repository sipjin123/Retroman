using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour {

    void OnTriggerStay(Collider hit)
    {
        Debug.LogError("Character STAY Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

    }
    void OnTriggerEnter(Collider hit)
	{
        Debug.LogError("Character Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

        if (LayerMask.LayerToName( hit.gameObject.layer) == "GroundOnly")
		{
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal());
		}
	}
}
