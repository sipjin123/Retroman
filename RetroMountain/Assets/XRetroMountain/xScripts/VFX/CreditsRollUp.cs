using UnityEngine;
using System.Collections;

public class CreditsRollUp : MonoBehaviour {

	float _yAxis;
	float _StartyAxis;
	float _EndyAxis;
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
				_yAxis += 3f;
			}
			else
			{
				_yAxis -=  3f;
			}
		}
		if(!Input.anyKey)
		{
			_yAxis += 1f;
		}

		transform.position = new Vector3(0, _yAxis,0);
	}
}
