using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour {

	#region VARIABLES
	private static PlatformManager _instance;
	public static PlatformManager Instance { get { return _instance; } }
	
	public enum SpawnDirection
	{
		FORWARD,
		LEFT,
		RIGHT,
		BACK
	}
	public SpawnDirection _spawnDirection;

	public GameObject _obstacleObject;
	public GameObject _DirectionFront, _DirectionBack, _DirectionLeft, _DirectionRight;

	public bool _turnAnyWhere;

	public GameObject _raySourceObject;
	public GameObject _rayLimitObject;
	public RaycastHit _rayObject;

	public GameObject _platformToSpawn;

	public GameObject _platformSpawnedParent;
	public GameObject _platformPoolParent;

	public int _randomizeChangeDirection;
	public int _randomizeChangeDirectionMin = 10;
	public int _randomizeChangeDirectionMax = 35;

	public int platformspassedforBalancing;
	public float _TOTALSPAWNEDPLATFORMS;
	public float _TOTALSPAWNEDPLATFORMSCAP;
	#endregion
	float StartCounter;
	#region INITIALIZER
	void Awake()
	{
		_instance = this;
	}
	void Start()
	{
		_randomizeChangeDirection = Random.Range( _randomizeChangeDirectionMin, _randomizeChangeDirectionMax );
		_turnAnyWhere = false;
		StartCounter = 0;
		_TOTALSPAWNEDPLATFORMS = 0;
		_TOTALSPAWNEDPLATFORMSCAP = 100;
		StartCoroutine(manualspawner());
	}
	#endregion

	
	//==================================================================================================================================================

	void Update()
	{
		if(Input.GetKey(KeyCode.X))
		{
			SpawnAPlatform();
		}
		if(Input.GetKey(KeyCode.Y))
		{
			StartCoroutine(manualspawner());
		}
	}
	IEnumerator manualspawner()
	{
		StartCounter++;
		yield return new WaitForSeconds(0.05f);
		SpawnAPlatform();
		if(StartCounter < 79)
		StartCoroutine(manualspawner());
	}
	//==================================================================================================================================================
	public void SpawnAPlatform()
	{
		if(Physics.Raycast(_raySourceObject.transform.position, -_raySourceObject.transform.up * 20, out _rayObject))
		{
			Debug.DrawRay( _raySourceObject.transform.position, -_raySourceObject.transform.up * 20, Color.red );
			if(_rayObject.collider.gameObject == _rayLimitObject)
			{
				//SPAWNS THE PLATFORM
				if(_platformPoolParent.transform.childCount < 2)
				{
					for(int i = 0 ; i < 5 ;i++)
					{
						GameObject tempx = Instantiate( _platformToSpawn, transform.position, Quaternion.identity ) as GameObject;
						tempx.SetActive(false);
						tempx.transform.parent = _platformPoolParent.transform;
						tempx.transform.position = _platformPoolParent.transform.position;
						tempx.name = "Ground";
					}
				}
				Transform temp = _platformPoolParent.transform.GetChild(0);
				temp.transform.position = new Vector3( _rayObject.point.x, -9, _rayObject.point.z );
				temp.gameObject.SetActive(true);
				temp.parent = _platformSpawnedParent.transform;
				_TOTALSPAWNEDPLATFORMS ++;
				//---------------------------
				if(_TOTALSPAWNEDPLATFORMS >= _TOTALSPAWNEDPLATFORMSCAP)
				{
					ObstacleManager.Instance.ProgressLevel();
					_TOTALSPAWNEDPLATFORMSCAP *= 1.5f;
				}

				
				//MOVES THE PLATFORM SPAWNER
				if(_spawnDirection == SpawnDirection.FORWARD)
				{
					transform.position += transform.forward * 1;
				}
				else if(_spawnDirection == SpawnDirection.BACK)
				{
					transform.position -= transform.forward * 1;
				}
				else if(_spawnDirection == SpawnDirection.RIGHT)
				{
					transform.position += transform.right * 1;
				}
				else if(_spawnDirection == SpawnDirection.LEFT)
				{
					transform.position -= transform.right * 1;
				}
				//---------------------------
				ReduceSpawnedPlatform();


				DirectionalCodes();
			}
		}
	}
	void ReduceSpawnedPlatform()
	{
		if(_platformPoolParent.transform.childCount < 2)
		{
			_platformSpawnedParent.transform.GetChild(0).GetComponent<PlatformObject>()._previousTypeOfPlatform = _platformSpawnedParent.transform.GetChild(0).GetComponent<PlatformObject>()._typeOfPlatform ;
			_platformSpawnedParent.transform.GetChild(0).GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.RECYCLE;
			_platformSpawnedParent.transform.GetChild(0).GetComponent<PlatformObject>().UpdateThisPlatform();
		}
	}
	void DirectionalCodes()
	{
		if(_turnAnyWhere)
		{
			GameObject temp1 = _platformPoolParent.transform.GetChild(0).gameObject;
			temp1.SetActive(true);
			if(_spawnDirection == SpawnDirection.FORWARD || _spawnDirection == SpawnDirection.BACK)
			{
				int _randomizer = Random.Range(0,2);

				if(_randomizer == 0)
				{
					if(_spawnDirection == SpawnDirection.BACK)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.LEFT;
					}
					if(_spawnDirection == SpawnDirection.FORWARD)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.RIGHT;
					}
					_obstacleObject.transform.position = _DirectionRight.transform.position;
					_obstacleObject.GetComponent<ObstacleManager>()._obstacleDirection = ObstacleManager.ObstacleDirection.RIGHT;
					_spawnDirection = SpawnDirection.RIGHT;
				}
				else
				{
					if(_spawnDirection == SpawnDirection.BACK)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.RIGHT;
					}
					if(_spawnDirection == SpawnDirection.FORWARD)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.LEFT;
					}
					_obstacleObject.transform.position = _DirectionLeft.transform.position;
					_obstacleObject.GetComponent<ObstacleManager>()._obstacleDirection = ObstacleManager.ObstacleDirection.LEFT;
					_spawnDirection = SpawnDirection.LEFT;
				}
			}
			else if (_spawnDirection == SpawnDirection.RIGHT || _spawnDirection == SpawnDirection.LEFT)
			{
				int _randomizer = Random.Range(0,2);

				if(_randomizer == 0)
				{
					if(_spawnDirection == SpawnDirection.RIGHT)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.LEFT;
					}
					if(_spawnDirection == SpawnDirection.LEFT)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.RIGHT;
					}
					_obstacleObject.transform.position = _DirectionFront.transform.position;
					_obstacleObject.GetComponent<ObstacleManager>()._obstacleDirection = ObstacleManager.ObstacleDirection.FORWARD;
					_spawnDirection = SpawnDirection.FORWARD;
				}
				else
				{
					if(_spawnDirection == SpawnDirection.RIGHT)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.RIGHT;
					}
					if(_spawnDirection == SpawnDirection.LEFT)
					{
						temp1.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.LEFT;
					}
					_obstacleObject.transform.position = _DirectionBack.transform.position;
					_obstacleObject.GetComponent<ObstacleManager>()._obstacleDirection = ObstacleManager.ObstacleDirection.BACKWARD;
					_spawnDirection = SpawnDirection.BACK;
				}
			}
			temp1.GetComponent<PlatformObject>().UpdateThisPlatform();
			
			_randomizeChangeDirection = Random.Range( _randomizeChangeDirectionMin, _randomizeChangeDirectionMax );
			ObstacleManager.Instance._nextMove = ObstacleManager.NextMove.COUNTNORMAL;
			ObstacleManager.Instance._obstacleDifficulty = (ObstacleManager.ObstacleDifficulty) Random.Range ( 0 , 3 );
			ObstacleManager.Instance.SetObstaclesToPass();


			ObstacleManager.Instance._turnCounter = 0;
			ObstacleManager.Instance._turnCounterToPass = Random.Range(50,150);

			ObstacleManager.Instance._obstaclesPassed = 0;

			_turnAnyWhere = false;
		}
	}
}
