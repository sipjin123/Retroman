using UnityEngine;
using System.Collections;

public class CharacterHitbox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Ground")
		{
			GameControls.Instance.GameOverIT();
		}
	}
}
