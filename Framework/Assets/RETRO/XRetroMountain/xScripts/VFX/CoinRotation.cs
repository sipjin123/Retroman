using UnityEngine;
using System.Collections;
using Synergy88;
using Common.Signal;

public class CoinRotation : MonoBehaviour {

	public bool _isPaused;
	// Use this for initialization
	void Start () {
		_isPaused = false;
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
	/*
	void FixedUpdate () {
		if(_isPaused)
			return;
		transform.Rotate(0,2,0);
	}*/
}
