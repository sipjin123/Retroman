using Retroman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroPlayerAI : MonoBehaviour {

    public GameObject _DetectorObject;
    public GameObject _DetectorObject1;
    float RayLength = 15;

    PlayerControls _PlayerControls;

    private void Start()
    {
        _PlayerControls = GetComponent<PlayerControls>();
        Assertion.AssertNotNull(_PlayerControls);
    }
    private void FixedUpdate()
    {
        RaycastHit ray;
        Debug.DrawRay(_DetectorObject.transform.position, -_DetectorObject.transform.up * RayLength, Color.yellow);
        //if (Physics.Raycast(RayObject.transform.position, -RayObject.transform.up * 10, out Rayhit))
        if (Physics.Raycast(_DetectorObject.transform.position,-_DetectorObject.transform.transform.up * RayLength ,out ray))
        {
            Debug.LogError("Hitting :: " + ray.collider.gameObject.name);
            if (ray.collider.GetComponent<PlatformMinion>() == null || ray.collider.gameObject.name == "FallStopper")
            {

                _PlayerControls.GenericJump();
            }
            else
            {
                Debug.LogError("Hitting :: " + ray.collider.gameObject.name + " " 
                    + ray.collider.gameObject.GetComponent<PlatformMinion>().TypeOfPlatform.ToString() + " "
                    + ray.collider.gameObject.GetComponent<PlatformMinion>().PlatformRunnability.ToString());
                PlatformMinion minionHit = ray.collider.gameObject.GetComponent<PlatformMinion>();
                if(minionHit.PlatformRunnability == PlatformRunnability.MustJump)
                {
                    _PlayerControls.GenericJump();
                }
                else
                {
                    Debug.LogError("Current is :: " + minionHit.transform.position.y + " Before is :: " + minionHit.PrePlatform.transform.position.y);
                    if(minionHit.transform.position.y > minionHit.PrePlatform.transform.position.y)
                    {
                        Debug.LogError("It Is Higher");
                        _PlayerControls.GenericJump();
                    }
                }
            }
        }
    }
}
