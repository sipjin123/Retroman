using UnityEngine;
using System.Collections;

public class CreditsRollUp : MonoBehaviour {

	float _yAxis;
	float _StartyAxis;
	float _EndyAxis;


	float _speed = 100;
	void OnEnable()
	{
		transform.position = new Vector3(0,0,0);
		_yAxis = transform.position.y;
	}

	void Update () {
		_yAxis = Mathf.Clamp(_yAxis , 0 , 1500);
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			_StartyAxis = Input.mousePosition.y;
		}
		if(Input.GetKey(KeyCode.Mouse0))
		{
			_EndyAxis = Input.mousePosition.y;

			if(_EndyAxis > _StartyAxis)
			{
				_yAxis += (_speed *Time.deltaTime);
			}
			else
			{
				_yAxis -=  (_speed *Time.deltaTime);
			}
		}
		if(!Input.anyKey)
		{
			_yAxis += ((_speed*0.25f) *Time.deltaTime);
		}

		transform.position = new Vector3(0, _yAxis,0);
	}
}
