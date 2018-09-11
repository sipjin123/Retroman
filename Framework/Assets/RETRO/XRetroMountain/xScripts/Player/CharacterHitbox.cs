using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour
{

    void OnTriggerStay(Collider hit)
    {
        return;
       // Debug.LogError("Character STAY Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

    }
    void OnTriggerEnter(Collider hit)
    {
       // return;
       // Debug.LogError("Character Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

        if(hit.GetComponent<PlatformMinion>() != null)//if (LayerMask.LayerToName(hit.gameObject.layer) == "GroundOnly")
        {
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal());
        }
    }
    void OnCollisionEnter(Collision hit)
    {
        return;
        //Debug.LogError("Character Hitbox COLL with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

        if(hit.gameObject.GetComponent<PlatformMinion>() != null)//if (LayerMask.LayerToName(hit.gameObject.layer) == "GroundOnly")
        {
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal());
        }
    }
}