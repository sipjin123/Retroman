using UnityEngine;
using System.Collections;

public class TriggerableObjects : MonoBehaviour {

	public enum TriggerState
	{
		TURNLEFT,
		TURNRIGHT,
		SPIKES,
		DEATHGROUND,
		STOPFALLING,
		IGNORE,
		ADDSCORE1,
		COINBOX,
		ENEMY,
		HOLE,
		SPRING,
		FALLBRIDGE,
		CATCHER,
		CATCHERTRIGGER

	}
	public TriggerState _triggerState;
	public enum EnemyState
	{
		DISABLED,
		WALK,
		TURNBACK,
		TURNFRONT
	}
	public EnemyState _enemyState;
	public GameObject _path1;
	public GameObject _path2;
	public GameObject _enemyObject;
	float _enemyBodyRotation;
	float _enemyBodyMoveSpeed = 0.05f;

	
	public GameObject[] _fallBridges;
	public bool _fallBridgeFall;

	
	public GameObject _catcherObject;
	public GameObject _catcherTrigger;
	public bool _triggerCatcher;
	
	public GameObject _coinObject;
	public GameObject _platformObject;

	void Awake()
	{
		
		_triggerCatcher = false;
	}
	void Start () 
	{
		_fallBridgeFall = false;
		_enemyBodyRotation = 0;
	}
	
	void Update () 
	{
		switch(_triggerState)
		{
			case TriggerState.ENEMY:
				if(_enemyState == EnemyState.DISABLED)
					return;
				if(_enemyState == EnemyState.TURNBACK)
				{
					transform.LookAt(_path2.transform.position);
					transform.position+=transform.forward * _enemyBodyMoveSpeed;
				}
				else if(_enemyState == EnemyState.TURNFRONT)
				{
					transform.LookAt(_path1.transform.position);
					transform.position+=transform.forward * _enemyBodyMoveSpeed;
				}
				break;

			case TriggerState.CATCHER:
				if(_triggerCatcher)
				{
					transform.position += transform.forward * 0.35f;
				}
				break;

			case TriggerState.FALLBRIDGE:
				if(_fallBridgeFall)
				{
					if(transform.position.y > -15)
					{
						transform.position += -transform.up * 0.25f;
					}
				}
				break;
		}
	}
	void OnEnable()
	{
		if(GetComponent<BoxCollider>() != null)
			GetComponent<BoxCollider>().enabled = true;
		if(_triggerState == TriggerState.CATCHER)
		{
			_triggerCatcher = true;
		}
	}
	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Player")
		{
			GetComponent<BoxCollider>().enabled = false;
			switch(_triggerState)
			{
				case TriggerState.CATCHERTRIGGER:
					_catcherObject.SetActive(true);
					break;

				case TriggerState.TURNRIGHT:
					PlayerControls.Instance._playerAction = PlayerControls.PlayerAction.TURNRIGHT;
					break;

				case TriggerState.TURNLEFT:
					PlayerControls.Instance._playerAction = PlayerControls.PlayerAction.TURNLEFT;
					break;

				case TriggerState.STOPFALLING:
					PlayerControls.Instance._playerAction = PlayerControls.PlayerAction.FALL;
					break;

				case TriggerState.SPRING:
					PlayerControls.Instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
					PlayerControls.Instance.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(25000, 40000));
					break;

				case TriggerState.ADDSCORE1:
					GameControls.Instance.Score += 1;
					if(transform.parent.gameObject.name == "Spikes")
						transform.parent.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
					break;

				case TriggerState.COINBOX:
					SoundControls.Instance._sfxBreakBlock.Play();
					GetComponent<MeshRenderer>().material.color = Color.black;
					_coinObject.SetActive(true);
					PlayerControls.Instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
					PlayerControls.Instance.gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * 7000);
					PlayerControls.Instance.isJumping = false;
					GameControls.Instance.Score += 1;
					break;
				
				case TriggerState.FALLBRIDGE:
					StartCoroutine (waitForBridgeFall());
					GetComponent<BoxCollider>().enabled = true;
					break;

				//DEATH
				case TriggerState.ENEMY:
					GameControls.Instance._gameState = GameControls.GameState.GAMEOVER;
					Debug.LogError("enemy killed you");
					break;
				case TriggerState.CATCHER:
					GameControls.Instance._gameState = GameControls.GameState.GAMEOVER;
					Debug.LogError("enemy killed you");
					break;
				case TriggerState.DEATHGROUND:
					GameControls.Instance._gameState = GameControls.GameState.GAMEOVER;
					Debug.LogError("hole killed you");
					break;
				case TriggerState.SPIKES:
					GameControls.Instance._gameState = GameControls.GameState.GAMEOVER;
					Debug.LogError("spikes killed you");
					break;

			}
		}
		if(_triggerState == TriggerState.ENEMY)
		{
			if(hit.gameObject.name == _path1.gameObject.name)
			{
				_enemyState = EnemyState.TURNBACK;
			}
			if(hit.gameObject.name == _path2.gameObject.name)
			{
				_enemyState = EnemyState.TURNFRONT;
			}
			if(hit.gameObject.tag == "Enemy")
				gameObject.SetActive(false);
		}
		if(_triggerState == TriggerState.CATCHER)
		{
			if(hit.gameObject.tag == "Enemy")
				gameObject.SetActive(false);
		}
	}
	public IEnumerator waitForBridgeFall()
	{
		yield return new WaitForSeconds (0.25f);
		_fallBridgeFall = true;
	}
}
