using UnityEngine;
using System.Collections;
using Synergy88;
using Common.Signal;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour {
	#region VARIABLES
	private static PlayerControls _instance;
	public static PlayerControls Instance { get { return _instance; } }
	public enum PlayerType
	{
		NORMAL,
		CAT,
		UNICORN,
		YOSHI,
		SONIC,
		DONKEYKONG
	}
	public PlayerType _playerType;
	public enum PlayerAction
	{
		TURNRIGHT,
		TURNLEFT,
		JUMP,
		FORWARD,
		FALL
	}
	public PlayerAction _playerAction;

	//RAYCAST 
	public bool isGrounded;
	public GameObject RayCastLimit;
	public GameObject RayObject;
	public RaycastHit Rayhit;


	//MOVEMENT
	public GameObject myMesh;
	public float movementSpeed;
	public float player_rotation;
	public float rotatedValue;

	public GameObject lerpToThisObject;

	//JUMP
	public bool isJumping;
	float rotateSpeed = 10;

	//EFFECTS
	public GameObject _shadowObject;
	public GameObject _deathAnim;
	public GameObject _splash;
	public GameObject _bodySpin;
	public float _bodySpinRotation;

	public GameObject _DKEffects;
	public GameObject _CatEffects;
	public GameObject _UnicornEffects;

	public bool _activePlayerObject;

	Rigidbody _rigidbody;
	//SPECIALS
	public bool _doubleJumpConsumed;
	float _walkCounter;


	bool _jumpDelaySwitch = false;

	#endregion
//==========================================================================================================================================
	#region INITIALIZATION
	void Awake()
	{
		_instance = this;
	}
	public void Start () 
	{
		isGrounded = true;

		movementSpeed = 0.15f;
		//movementSpeed = 0.15f;
		_rigidbody = GetComponent<Rigidbody>();
		_bodySpinRotation = 0;
		_activePlayerObject = false;
		_playerAction = PlayerAction.FORWARD;
		rotatedValue = 0;
		player_rotation = 0;
		isJumping = false;
		Time.timeScale = 1.5f;

		_doubleJumpConsumed = false;

		//COSTUME
		_bodySpin.transform.GetChild((int)_playerType).gameObject.SetActive(true);
		_deathAnim.transform.GetChild(0).GetChild((int)_playerType).gameObject.SetActive(true);

		SetupPlayerType( PlayerPrefs.GetInt("CurrentCharacter", 0) );


		_rigidbody.useGravity = false;
	}
	#endregion
	public void SetupPlayerType(int _playaTyp)
	{
		_playerType = (PlayerType)_playaTyp;

		for(int i = 0 ; i < _bodySpin.transform.childCount ; i++)
		{
			_bodySpin.transform.GetChild(i).gameObject.SetActive(false);
			_deathAnim.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
		}
		PlayerPrefs.SetInt("CurrentCharacter", (int)_playerType);
		_bodySpin.transform.GetChild((int)_playerType).gameObject.SetActive(true);
		_deathAnim.transform.GetChild(0).GetChild((int)_playerType).gameObject.SetActive(true);
	}
	public void REstarer()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}
//==========================================================================================================================================
	void FixedUpdate () 
	{
		if(!_activePlayerObject)
			return;
		


		//if(Input.GetKey(KeyCode.W))
		if(_playerAction == PlayerAction.FORWARD)
		{
			transform.position += (transform.forward * movementSpeed)  ;
			_walkCounter += movementSpeed;
			if(_walkCounter > 1)
			{
				PlatformLord.Instance.SpawnAPlatform();
				_walkCounter --;
			}
		}
		//JUMP START
		if(!isJumping && isGrounded)
		{
			if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
			{
				_jumpDelaySwitch = true;
				StartCoroutine(JumpDelayENUM());
				_rigidbody.AddForce(transform.up * 15000);
				isJumping = true;

				RayCastLimit.SetActive(false);
				StartCoroutine(waitforRaycastActive());
				SoundControls.Instance._sfxJump.Play();
				return;

			}
		}
		else //JUMP SPECIAL
		{
			if(isJumping)
			{

				if((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)))
				{
					_rigidbody.AddForce(transform.up * 650);
				}
			}
		}

			_rigidbody.AddForce(-transform.up *1000);

		PlayerTurnFunction();
		RaycastFunction();

	}
	//==========================================================================================================================================	
	#region COROUTINES
	IEnumerator JumpDelayENUM()
	{
		yield return new WaitForSeconds(0.25f);
		_jumpDelaySwitch = false;
	}
	IEnumerator waitforJumpActive(float _time)
	{
		yield return new WaitForSeconds(_time);
		_doubleJumpConsumed = true;
	}
	IEnumerator waitforRaycastActive()
	{
		yield return new WaitForSeconds(0.05f);
		RayCastLimit.SetActive(true);
	}
	#endregion
	//==========================================================================================================================================
	#region RAYCAST
	void RaycastFunction()
	{
		if(Physics.Raycast(RayObject.transform.position, -RayObject.transform.up * 1f, out Rayhit ))
		{
			//Debug.LogError(Rayhit.collider.gameObject.tag+" "+Rayhit.collider.gameObject.name);
			//Debug.DrawRay(RayObject.transform.position, -RayObject.transform.up * 1, Color.red);
			if(Rayhit.collider.gameObject.tag == "Ground")
			{
				_shadowObject.transform.position = new Vector3(  _shadowObject.transform.position.x, Rayhit.point.y, _shadowObject.transform.position.z);
			}
			if(isJumping)
			{
				if(Rayhit.collider.gameObject.name == "FallStopper")
				{
					_shadowObject.SetActive(false);
				}
				else if(Rayhit.collider.gameObject.tag == "Ground") 
				{
					_shadowObject.SetActive(true);
				}
			}
		}
	}
	#endregion
	//==========================================================================================================================================
	#region FUNCTIONS
	void PlayerTurnFunction()
	{
		if(_playerAction == PlayerAction.TURNLEFT)
		{
			player_rotation -=  rotateSpeed;
			rotatedValue += rotateSpeed;
			if(rotatedValue >=90)
			{
				_playerAction = PlayerAction.FORWARD;
				rotatedValue = 0;
				CameraControls.Instance.SwitchCam(180);
			}
			transform.eulerAngles = new Vector3( 0,player_rotation,0 );
			LerpToPosition( lerpToThisObject, lerpToThisObject.transform.position , 0);
		}
		else if(_playerAction == PlayerAction.TURNRIGHT)
		{
			player_rotation += rotateSpeed;
			rotatedValue += rotateSpeed;
			if(rotatedValue >=90)
			{
				_playerAction = PlayerAction.FORWARD;
				rotatedValue = 0;
				CameraControls.Instance.SwitchCam(0);
			}
			transform.eulerAngles = new Vector3( 0,player_rotation,0 );
			LerpToPosition( lerpToThisObject, lerpToThisObject.transform.position , 0);
		}
	}
	void LerpToPosition(GameObject _targetPosition, Vector3 StartPos, float _targetTime)
	{
		_targetTime += 0.01f;
		transform.position = Vector3.Lerp(new Vector3(StartPos.x, transform.position.y, StartPos.z), new Vector3 (_targetPosition.transform.position.x , transform.position.y , _targetPosition.transform.position.z), _targetTime);
		if(_targetTime < 1)
			LerpToPosition(_targetPosition, StartPos, _targetTime);
	}
	#endregion
	//==========================================================================================================================================
	#region COLLISIONS
	void OnTriggerEnter(Collider hit)
	{
		//TURNING
		if(hit.gameObject.name == "Left" || hit.gameObject.name == "Right")
		{
			lerpToThisObject = hit.gameObject;
			if(hit.gameObject.name == "Left")
				_playerAction = PlayerAction.TURNLEFT;
			if(hit.gameObject.name == "Right")
				_playerAction = PlayerAction.TURNRIGHT;
			hit.gameObject.SetActive(false);
		}
		//FALLING
		else if(hit.gameObject.name == "FallStopper")
		{
			_playerAction = PlayerAction.FALL;
		}
		//JUMPING
		if(!_jumpDelaySwitch)
		{
			if(hit.gameObject.tag == "Ground")
			{
				isGrounded = true;
				if(isJumping)
				{
					_rigidbody.velocity = new Vector3(0,0,0);
				}
				isJumping = false;
			}
		}
	}
	#endregion
	//==========================================================================================================================================
	#region SIGNALS
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
		_activePlayerObject = false;
		_rigidbody.useGravity = false;
	}
	void OnGameResume(ISignalParameters parameters)
	{
		_shadowObject.SetActive(true);
		_activePlayerObject = true;
		_rigidbody.useGravity = true;
		CameraControls.Instance._startFollow = true;
	}
	#endregion
}
