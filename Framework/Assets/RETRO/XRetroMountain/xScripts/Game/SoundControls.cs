 using UnityEngine;
using System.Collections;
using Common.Utils;

using UniRx;
using Retroman;
public class SoundControls : MonoBehaviour {
	private static SoundControls _instance;
	public static SoundControls Instance { get { return _instance; } }


	public AudioSource _sfxJump, _sfxBreakBlock, _sfxDie, _sfxBGM, _sfxCoin, _sfxRunning, _sfxSpikes, _sfxSplash, _buttonClick;

	public GameObject _BGMParent,_SFXParent;

	bool _bgmswitch, sfxswitch;
	void Awake()
	{
		_instance = this;
	}
    public void SetupMessageBroker(MessageBroker broker)
    {
        broker.Receive<LaunchGamePlay>().Subscribe(_ =>
        {
            REsetupBGM();


        }).AddTo(this);
    }

	void OnEnable()
	{
		SetUpSounds();
	}
	public void SetUpSounds()
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

    public void PauseBGM()
    {
        _sfxBGM.Pause();
    }
    public void REsumeBGM()
    {
        _sfxBGM.UnPause();
    }

    public void REsetupBGM()
    {

        if (PlayerPrefs.GetInt("BGMSWITCH", 1) == 1)
        {
            _sfxBGM.gameObject.SetActive(true);
            _sfxBGM.time = 0;
            _sfxBGM.Play();
        }
    }
    
}
