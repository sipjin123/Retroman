 using UnityEngine;
using System.Collections;

public class SoundControls : MonoBehaviour {
	private static SoundControls _instance;
	public static SoundControls Instance { get { return _instance; } }


	public AudioSource _sfxJump, _sfxBreakBlock, _sfxDie, _sfxBGM;
	void Awake()
	{
		_instance = this;
	}

	void Start () 
	{
	
	}

	void Update () 
	{
	
	}
}
