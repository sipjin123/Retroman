using UnityEngine;
using System.Collections;
public class CameraControls : MonoBehaviour {
	private static CameraControls _instance;
	public static CameraControls Instance { get { return _instance; } }

	public GameObject PlayerObject;
	public float[] RandomizedRotationAxis;
	public GameObject _LeftPosition, _RightPosition ,_midPosition;
	public GameObject _CamObj;

	bool _lerpToCamSwitch;
	Vector3  _oBjToLerpTo, _objtoLerpFrom;

	public bool _startFollow;

	void Awake()
	{
		_instance = this;
	}
	void Start () 
	{
		_lerpToCamSwitch = false;
		_startFollow = false;
	}

	void FixedUpdate () {
		if(_startFollow)
		transform.position = new Vector3 ( PlayerObject.transform.position.x, transform.position.y, PlayerObject.transform.position.z );
	
		if(_lerpToCamSwitch)
		{
			LerpThoThisCam();
		}
	}
	public void SwitchCam(int _direction)
	{
		if(_direction == 180 )
		{
			_oBjToLerpTo = _LeftPosition.transform.localPosition;
			_objtoLerpFrom = _CamObj.transform.localPosition;
			_lerpToCamSwitch = true;
		}
		else if(_direction == 0)
		{
			_oBjToLerpTo = _RightPosition.transform.localPosition;
			_objtoLerpFrom = _CamObj.transform.localPosition;
			_lerpToCamSwitch = true;
		}
		else if(_direction == 2)
		{
			_oBjToLerpTo = _midPosition.transform.localPosition;
			_objtoLerpFrom = _CamObj.transform.localPosition;
			_lerpToCamSwitch = true;
		}
	}

	float _lerpValue;
	public void LerpThoThisCam()
	{
		if(_lerpValue < 1)
		{
			_lerpValue += 1.75f * Time.deltaTime;	
			_CamObj.transform.localPosition = Vector3.Lerp(_objtoLerpFrom, _oBjToLerpTo, _lerpValue);
		}
		else
		{
			_lerpToCamSwitch = false;

			_lerpValue = 0;
			return;
		}
	}
}
