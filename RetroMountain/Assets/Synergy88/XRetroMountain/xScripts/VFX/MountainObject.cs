using UnityEngine;
using System.Collections;
using Synergy88;
using Common.Signal;

public class MountainObject : MonoBehaviour {

	public GameObject _objA, _objB;
	public bool _isPaused;
	// Use this for initialization
	void Start () {
		_isPaused  = false;
	}

	void OnEnable()
	{
		S88Signals.ON_GAME_PAUSE.AddListener(OnGamePause);
		S88Signals.ON_GAME_RESUME.AddListener(OnGameResume);
	}
	void OnDisable()
	{
		S88Signals.ON_GAME_PAUSE.RemoveListener(OnGamePause);
		S88Signals.ON_GAME_RESUME.RemoveListener(OnGameResume);
	}
	void OnGamePause(ISignalParameters parameters)
	{
		_isPaused = true;
	}
	void OnGameResume(ISignalParameters parameters)
	{
		_isPaused = false;
	}

	// Update is called once per frame
	/*void FixedUpdate () {
	
		if(_isPaused)
			return;
		transform.position -= transform.forward * 0.05f;
	}*/
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.name == "AntiMountain")
		{
			if(gameObject.name == "A")
				transform.position = _objB.transform.GetChild(0).transform.position;
			if(gameObject.name == "B")
				transform.position = _objA.transform.GetChild(0).transform.position;
		}
	}
}
