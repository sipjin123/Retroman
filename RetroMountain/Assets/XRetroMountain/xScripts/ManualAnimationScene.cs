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

	// Use this for initialization
	void Start () {
		_waypoint = 1;
		_startLerping = true;
		_lerpValue = 0;
		_thispos = _wayPoints[0].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.K))
		{
			
			_startLerping = true;
			_thispos = _wayPoints[0].transform.position;
		}
		if(_startLerping)
		{
			if(_lerpValue > 1)
			{

				_startLerping = false;
				StartCoroutine(LerpWWait());
			}
			_lerpValue += Time.deltaTime * 2.75f;
			try{
			transform.position = Vector3.Lerp(_thispos,  _wayPoints[_waypoint].transform.position , _lerpValue );
			}
			catch{}
		}

	}
	IEnumerator LerpWWait()
	{
		yield return new WaitForSeconds(0.25f);
		try{
		_waypoint++;
		_thispos = transform.position;
		_lerpValue = 0;
		_startLerping = true;
		}
		catch{
			_startLerping = false;
		}
	}
}
