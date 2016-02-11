using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

	public GameObject PlayerObject;
	public float[] RandomizedRotationAxis;
	// Use this for initialization
	void Start () {
		transform.eulerAngles = new Vector3( 0, RandomizedRotationAxis[Random.Range(0,4)],0 );
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.position = new Vector3 ( PlayerObject.transform.position.x, transform.position.y, PlayerObject.transform.position.z );
	}
}
