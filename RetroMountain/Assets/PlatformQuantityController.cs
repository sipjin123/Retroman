using UnityEngine;
using System.Collections;

public class PlatformQuantityController : MonoBehaviour {

	public GameObject _frontDetectorStart, _frontDetectorLimit;
	public GameObject _backDetectorStart, _backDetectorLimit;

	public RaycastHit _frontRay, _backRay;
	float platCounter;
	void Start () 
	{
		platCounter = PlatformManager.Instance.platformspassedforBalancing;
	}

	void OnDisable()
	{
		GetComponent<BoxCollider>().enabled = true;
	}
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Player")
		{
			PlatformManager.Instance.platformspassedforBalancing ++;
			if(PlatformManager.Instance.platformspassedforBalancing > 50)
			{
				PlatformManager.Instance.SpawnAPlatform();
			}
			GetComponent<BoxCollider>().enabled = false;
		}
	}
}
