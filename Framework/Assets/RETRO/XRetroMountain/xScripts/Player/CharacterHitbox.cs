using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour
{
    public bool LogMe;
    private void Start()
    {
        LogMe = false;
    }
    void OnTriggerStay(Collider hit)
    {
        if(LogMe)
        Debug.LogError("I am Staying with :: " + hit.gameObject.name);
        return;
       // Debug.LogError("Character STAY Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));

    }
    void OnTriggerEnter(Collider hit)
    {
        // return;
        // Debug.LogError("Character Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));
        Debug.LogError("I am Hitting :: " + hit.gameObject.name);
        if(hit.GetComponent<PlatformMinion>() != null || hit.gameObject.name == "FallStopper")//if (LayerMask.LayerToName(hit.gameObject.layer) == "GroundOnly")
        {
            Debug.LogError("Game OVer");
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