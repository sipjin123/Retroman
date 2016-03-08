 using UnityEngine;
using System.Collections;

public class SoundControls : MonoBehaviour {
	private static SoundControls _instance;
	public static SoundControls Instance { get { return _instance; } }


	public AudioSource _sfxJump, _sfxBreakBlock, _sfxDie, _sfxBGM, _sfxCoin, _sfxRunning, _sfxSpikes, _sfxSplash;

	public GameObject _BGMParent,_SFXParent;

	bool _bgmswitch, sfxswitch;
	void Awake()
	{
		_instance = this;
	}
	void OnEnable()
	{
		if(PlayerPrefs.GetInt("BGMSWITCH",1) == 0)
			_bgmswitch = false;
		else
			_bgmswitch = true;

		if(PlayerPrefs.GetInt("SFXSWITCH",1) == 0)
			sfxswitch = false;
		else
			sfxswitch = true;


		foreach(Transform child in _BGMParent.transform)
		{
			child.GetComponent<AudioSource>().mute = !_bgmswitch;
		}
		foreach(Transform child in _SFXParent.transform)
		{
			child.GetComponent<AudioSource>().mute = !sfxswitch;
		}
	}
}
