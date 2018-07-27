using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;

public class CharacterHitbox : MonoBehaviour {

	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Ground")
		{
			Factory.Get<DataManagerService>().GameControls.GameOverIT();
		}
	}
}
