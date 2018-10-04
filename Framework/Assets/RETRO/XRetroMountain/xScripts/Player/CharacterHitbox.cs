using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour
{
    bool LogMe;
    private void Start()
    {
        LogMe = false;
    }
    void OnTriggerStay(Collider hit)
    {
        if(LogMe)
        Debug.LogError("I am Staying with :: " + hit.gameObject.name);
        //return;
        // Debug.LogError("Character STAY Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));
        if (hit.GetComponent<PlatformMinion>() != null || hit.gameObject.name == "FallStopper")//if (LayerMask.LayerToName(hit.gameObject.layer) == "GroundOnly")
        {
            Debug.LogError("Game OVer");
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            if(hit.GetComponent<PlatformMinion>() != null)
                Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal { KilledBy = "Platform" });
            else
                Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal {KilledBy = hit.gameObject.name });
        }
    }
    void OnTriggerEnter(Collider hit)
    {
        // return;
        // Debug.LogError("Character Hitbox Colliding with :: " + hit.gameObject.name + " " + LayerMask.LayerToName(hit.gameObject.layer));
        if (LogMe)
            Debug.LogError("I am Hitting :: " + hit.gameObject.name);
        if(hit.GetComponent<PlatformMinion>() != null || hit.gameObject.name == "FallStopper")//if (LayerMask.LayerToName(hit.gameObject.layer) == "GroundOnly")
        {
            Debug.LogError("Game OVer");
            Factory.Get<VFXHandler>().RequestVFX(hit.transform.position, VFXHandler.VFXList.BumpVFX);
            if (hit.GetComponent<PlatformMinion>() != null)
                Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal { KilledBy = "Platform" });
            else
                Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOverSignal { KilledBy = hit.gameObject.name });
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