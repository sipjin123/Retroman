using UnityEngine;
using System.Collections;

public class ManualAnimationScene : MonoBehaviour {

	public enum KinOfObj
	{
		PLAYER,
		CAMERA
	}
	public KinOfObj _kindOf;

	public GameObject[] _wayPoints;

	float _lerpValue;
	bool _startLerping;
	int _waypoint;

	Vector3 _thispos;
    public GameObject[] shadowsToDisable;
	// Use this for initialization
	void Start () {
		_waypoint = 1;
		_startLerping = true;
		_lerpValue = 0;
		_thispos = _wayPoints[0].transform.position;
		_distLenght = Vector3.Distance( _thispos , _wayPoints[_waypoint].transform.position);
		_startTime = Time.time;/////
	}
	
	// Update is called once per frame
	void Update () {
		if(_waypoint > 4)
			GetComponent<ManualAnimationScene>().enabled = (false);
		if(Input.GetKeyDown(KeyCode.K))
		{
			
			_startLerping = true;
			_thispos = _wayPoints[0].transform.position;
			_distLenght = Vector3.Distance( _thispos , _wayPoints[_waypoint].transform.position);
			_startTime = Time.time;/////
		}
		if(_startLerping)
		{
			if(_lerpValue > 1)
			{

				_startLerping = false;
				StartCoroutine(LerpWWait());
			}
			//_lerpValue += Time.deltaTime * 2.75f;
			float _distCov = (Time.time - _startTime) * _moveSpeed;
			_lerpValue =  _distCov / _distLenght;
				
			try{
			transform.position = Vector3.Lerp(_thispos,  _wayPoints[_waypoint].transform.position , _lerpValue );
			}
			catch{}
		}
        if (_waypoint == 1)
            shadowsToDisable[0].SetActive(false);
        if (_waypoint == 2)
            shadowsToDisable[1].SetActive(true);
        if (_waypoint == 3)
            shadowsToDisable[1].SetActive(false);

    }
	float _distLenght;
	float _startTime;
	float _moveSpeed = 10;
	IEnumerator LerpWWait()
	{
		yield return new WaitForSeconds(0.15f);
		try{
		_waypoint++;
		_thispos = transform.position;
		_lerpValue = 0;
			_startLerping = true;
			_distLenght = Vector3.Distance( transform.position , _wayPoints[_waypoint].transform.position);/////
			_startTime = Time.time;/////
		}
		catch{
			_startLerping = false;
		}
	}
}
