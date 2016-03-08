using UnityEngine;
using System.Collections;

public class CreditsRollUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnEnable()
	{
		transform.position = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.up *1f;
	}
}
