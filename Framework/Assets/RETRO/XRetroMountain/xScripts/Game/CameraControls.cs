using UnityEngine;
using System.Collections;
using Common.Utils;
using Retroman;
using UniRx;
public class CameraControls : MonoBehaviour {

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
        Factory.Get<DataManagerService>().MessageBroker.Receive<ChangeCamAngle>().Subscribe(_ => 
        {
            Debug.LogError("NEED TO LERP");
            _ResetToDirection((int)_.Angle);
        }).AddTo(this);
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
        Debug.LogError("------------------------------------------------------------------------------ SWITCHING CAM TO :: " + _direction);
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
    private void saddaOnGUI()
    {
        GUI.Box(new Rect(0, 0, 100, 30), ""+_lerpValue);
        GUI.Box(new Rect(0, 30, 100, 30), "" + _lerpToCamSwitch);
    }
}
