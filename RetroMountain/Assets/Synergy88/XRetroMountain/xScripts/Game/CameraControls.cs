using UnityEngine;
using System.Collections;
public class CameraControls : MonoBehaviour {
	private static CameraControls _instance;
	public static CameraControls Instance { get { return _instance; } }

	public GameObject PlayerObject;
	public float[] RandomizedRotationAxis;
	public GameObject _LeftPosition, _RightPosition;
	public GameObject _CamObj;

	bool _lerpToCamSwitch;
	Vector3  _oBjToLerpTo, _objtoLerpFrom;

	float _lerpSpeed;
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
	public void _ResetToDirection(int _direction)
	{
		_lerpToCamSwitch = false;
		_lerpValue = 0;
		SwitchCam(_direction);
	}
	public void SwitchCam(int _direction)
	{
		if(_direction == 180 )
		{
			_oBjToLerpTo = _LeftPosition.transform.localPosition;
			_objtoLerpFrom = _CamObj.transform.localPosition;
			_lerpToCamSwitch = true;
			_lerpSpeed =  1f;
		}
		else if(_direction == 0)
		{
			_oBjToLerpTo = _RightPosition.transform.localPosition;
			_objtoLerpFrom = _CamObj.transform.localPosition;
			_lerpToCamSwitch = true;
			_lerpSpeed =  1f;
		}
	}

	float _lerpValue;
	public void LerpThoThisCam()
	{
		if(_lerpValue < 1)
		{
			_lerpValue += _lerpSpeed * Time.deltaTime;	
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
