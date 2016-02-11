using UnityEngine;
using System.Collections;

public class EnemyControls : MonoBehaviour {

	public enum EnemyState
	{
		RIGHT,
		LEFT,
		DIE
	}
	public EnemyState _enemyState;
	public float EnemyMovement;
	// Use this for initialization
	void Start () {
	
		EnemyMovement = 0.05f;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(_enemyState == EnemyState.RIGHT)
		{
			transform.position += transform.right * EnemyMovement;
			transform.localEulerAngles = new Vector3 (0,-90,0);
		}
		else if(_enemyState == EnemyState.LEFT)
		{
			transform.position += transform.right * EnemyMovement;
			transform.localEulerAngles = new Vector3 (0,90,0);
		}

	}
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.name == "PointB")
		{
			_enemyState = EnemyState.LEFT;
		}
		else if(hit.gameObject.name == "PointA")
		{
			_enemyState = EnemyState.RIGHT;
		}
	}
}
