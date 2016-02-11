using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	private static PlayerControls _instance;
	public static PlayerControls Instance { get { return _instance; } }
	public bool isJumping;
	public float jumpValue;
	float jumpValueMax = 20;
	float rotateSpeed = 30;

	public GameObject RayObject;
	public RaycastHit Rayhit;
	public GameObject RayObject2;
	public RaycastHit Rayhit2;

	public float movementSpeed;
	public float player_rotation;
	public float rotatedValue;
	public GameObject myMesh;

	public enum PlayerAction
	{
		TURNRIGHT,
		TURNLEFT,
		JUMP,
		FORWARD,
		FALL
	}
	public PlayerAction _playerAction;

	public bool isGrounded;
	public bool isGrounded2;
	public GameObject RayCastLimit;
	public GameObject RayCastLimit2;
	public bool _activePlayerObject;
//==========================================================================================================================================
	void Awake()
	{
		_instance = this;
	}
	void Start () 
	{
		_activePlayerObject = false;
		movementSpeed = 0.2f;
		_playerAction = PlayerAction.FORWARD;
		rotatedValue = 0;
		player_rotation = 0;
		isJumping = false;
		jumpValue = jumpValueMax;
		Time.timeScale = 2;
	}
//==========================================================================================================================================
	void Update () 
	{
		if(!_activePlayerObject)
			return;

		//if(Input.GetKey(KeyCode.W))
		if(_playerAction == PlayerAction.FORWARD)
		{
				transform.position += (transform.forward * movementSpeed);
		}
		if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && !isJumping && (isGrounded || isGrounded2))
		{
			RayCastLimit.SetActive(false);
			RayCastLimit2.SetActive(false);
			GetComponent<Rigidbody>().AddForce(transform.up * 15000);
			StartCoroutine(waitforRaycastActive());
			isJumping = true;
			SoundControls.Instance._sfxJump.Play();
		}
		if((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && isJumping)
		{
			GetComponent<Rigidbody>().AddForce(transform.up * 550);
		}
		GetComponent<Rigidbody>().AddForce(-transform.up *1000);

		PlayerTurnFunction();
		RaycastFunction();
	}
//==========================================================================================================================================
	IEnumerator waitforRaycastActive()
	{
		yield return new WaitForSeconds(0.25f);
		RayCastLimit.SetActive(true);
		RayCastLimit2.SetActive(true);
	}
	void RaycastFunction()
	{
		//Debug.LogError(isGrounded+" "+isGrounded2+" "+isJumping);
		if(Physics.Raycast(RayObject.transform.position, -RayObject.transform.up * 1f, out Rayhit ))
		{
			//Debug.LogError(Rayhit.collider.gameObject.tag+" "+Rayhit.collider.gameObject.name+" "+transform.position.y);
			Debug.DrawRay(RayObject.transform.position, -RayObject.transform.up * 1, Color.red);
			if(!isJumping)
			{
				if(Rayhit.collider.gameObject.name == "RayEnd") 
				{
					isGrounded = false;
				}
				else
				{	
					isGrounded = true;
				}
			}
			else if(isJumping)
			{
				if(Rayhit.collider.gameObject.tag == "Ground" && RayCastLimit.activeInHierarchy ) 
				{
						isJumping = false;
						isGrounded = true;
				}
			}
		}
		if(Physics.Raycast(RayObject2.transform.position, -RayObject2.transform.up * 1f, out Rayhit2 ))
		{
			//Debug.LogError(Rayhit.collider.gameObject.name);
			Debug.DrawRay(RayObject2.transform.position, -RayObject2.transform.up * 1, Color.red);
			if(!isJumping)
			{
				if(Rayhit2.collider.gameObject.name == "RayEnd2") 
				{
					isGrounded2 = false;
				}
				else
				{	
					isGrounded2 = true;
				}
			}
			else if (isJumping)
			{
				if(Rayhit2.collider.gameObject.tag == "Ground" && RayCastLimit2.activeInHierarchy ) 
				{
					isJumping = false;
					isGrounded = true;
				}
			}
		}
	}
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
			}
			transform.eulerAngles = new Vector3( 0,player_rotation,0 );
			LerpToPosition( lerpToThisObject, lerpToThisObject.transform.position , 0);
		}
	}
	
	public GameObject lerpToThisObject;
	void LerpToPosition(GameObject _targetPosition, Vector3 StartPos, float _targetTime)
	{
		_targetTime += 0.01f;
		transform.position = Vector3.Lerp(new Vector3(StartPos.x, transform.position.y, StartPos.z), new Vector3 (_targetPosition.transform.position.x , transform.position.y , _targetPosition.transform.position.z), _targetTime);
		if(_targetTime < 1)
			LerpToPosition(_targetPosition, StartPos, _targetTime);
	}

	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.name == "Left" || hit.gameObject.name == "Right")
			lerpToThisObject = hit.gameObject;
		if(hit.gameObject.name == "Blocker")
			hit.gameObject.transform.parent.GetComponent<SphereCollider>().enabled = false;
	}
}
