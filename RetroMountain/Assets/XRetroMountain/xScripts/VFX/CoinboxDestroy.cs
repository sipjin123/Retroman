using UnityEngine;
using System.Collections;

public class CoinboxDestroy : MonoBehaviour {

	Vector3[] origstatepos;
	// Use this for initialization
	void Start () {
		origstatepos = new Vector3[transform.childCount];
		for(int i = 0 ; i < transform.childCount ; i++)
		{
			origstatepos[i] = transform.GetChild(i).gameObject.transform.localPosition;
		}
	}
	void OnDisable()
	{
		for(int i = 0 ; i < transform.childCount ; i++)
		{
			transform.GetChild(i).gameObject.transform.localPosition = origstatepos[i];
			transform.GetChild(i).gameObject.transform.localRotation = Quaternion.identity;
			//transform.localScale = new Vector3(1,1,1);
			try{
			transform.GetChild(i).GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			}catch{}
		}
	}
	void OnEnable()
	{
		for(int i = 0 ; i < transform.childCount ; i++)
		{
			try{
				transform.GetChild(i).gameObject.transform.localPosition = origstatepos[i];
				transform.GetChild(i).gameObject.transform.localRotation = Quaternion.identity;
			//transform.localScale = new Vector3(1,1,1);
			}
			catch{}
		}
	}
}
