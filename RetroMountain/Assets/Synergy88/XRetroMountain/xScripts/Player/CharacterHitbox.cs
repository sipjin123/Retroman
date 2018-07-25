using UnityEngine;
using System.Collections;

public class CharacterHitbox : MonoBehaviour {

	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Ground")
		{
			GameControls.Instance.GameOverIT();
		}
	}
}
