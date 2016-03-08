using UnityEngine;
using System.Collections;

public class Trigger_Master: MonoBehaviour {


	public enum TypeOfTrigger{
		COIN,
		SPIKE,
		LEFT,
		RIGHT,
		COUNTER,
		WATER
	}
	public TypeOfTrigger _typeOfTrigger;

	public GameObject _coinMesh, _coinEffects;

	void OnEnable()
	{
		try{
		_coinMesh.SetActive(true);
		_coinEffects.SetActive(false);
		}catch{}
	}
	void OnTriggerEnter(Collider hit)
	{
		if(hit.tag == "Player")
		{
			switch(_typeOfTrigger)
			{
				case TypeOfTrigger.COIN:
						_coinMesh.SetActive(false);
						_coinEffects.SetActive(true);
						if(gameObject.name == "CoinBox")
						{
							SoundControls.Instance._sfxBreakBlock.Play();
						}
						if(gameObject.name == "CoinOBJ")
						{
							SoundControls.Instance._sfxCoin.Play();
							GameControls.Instance.Score++;
							GameControls.Instance.UptateScoreing();
						}
					break;
				case TypeOfTrigger.SPIKE:
					GameControls.Instance.GameOverIT();
					SoundControls.Instance._sfxSpikes.Play();
					break;
				case TypeOfTrigger.LEFT:
					PlayerControls.Instance._playerAction = PlayerControls.PlayerAction.TURNLEFT;
					break;
				case TypeOfTrigger.RIGHT:
					PlayerControls.Instance._playerAction = PlayerControls.PlayerAction.TURNLEFT;
				break;
				case TypeOfTrigger.WATER:
						SoundControls.Instance._sfxSplash.Play();
						PlayerControls.Instance._splash.SetActive(true);
				PlayerControls.Instance._splash.transform.position = PlayerControls.Instance._deathAnim.transform.GetChild(0).transform.position;
						GameControls.Instance.GameOverIT();
					break;
				case TypeOfTrigger.COUNTER:
						PlatformLord.Instance.SpawnAPlatform();
					break;
			}
		}

	}
}
