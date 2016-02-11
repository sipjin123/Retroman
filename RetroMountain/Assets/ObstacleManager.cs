using UnityEngine;
using System.Collections;

public class ObstacleManager : MonoBehaviour {
	private static ObstacleManager _instance;
	public static ObstacleManager Instance { get { return _instance; } }

	public GameObject _raySourceObject;
	public GameObject _rayLimitObject;
	public RaycastHit _rayObject;

	public enum StackableObstacle
	{
		NONE,
		SPIKED,
		HOLED,
		COINED,
		SPRING,
		ENEMY,
		FALLBRIDGE,
		CATCHER
	}
	public StackableObstacle _stackableObstacle;
	public enum NextMove
	{
		COUNTNORMAL,
		SPAWNOBSTACLE,
		TURNANYWHERE
	}
	public NextMove _nextMove;
	public enum ObstacleDifficulty
	{
		EASY,
		MEDIUM,
		HARD
	}
	public ObstacleDifficulty _obstacleDifficulty;

	
	public enum ObstacleDirection
	{
		FORWARD,
		BACKWARD,
		RIGHT,
		LEFT
	}
	public ObstacleDirection _obstacleDirection;


	[SerializeField]
	int[] EasyValues, MediumValues, HardValues;
	public int _LevelProgression;


	public int _totalStacks;
	int _maxStacks = 4;

	public int _obstaclesPassed;
	public int _obstaclesToPass;
	public int _fixedPattern;
	public int _turnCounter;
	public int _turnCounterToPass;

	public int _maxObstacles;
	public GameObject _enemyToSpawn;
	public GameObject _fallbridgeToLookat;
	void Awake()
	{
		_instance = this;
	}
	void Start () 
	{
		_maxObstacles = 4;//_maxObstacles = 4;
		_LevelProgression = 0;
		_turnCounter = 0;
		_turnCounterToPass = Random.Range(50,150);
		_obstaclesPassed = 0;
		_totalStacks = 0;
		_fixedPattern = 1;
	}

	void Update () 
	{
		SpawnAnObstacle();
	}

	public void SpawnAnObstacle()
	{
		if(Physics.Raycast(_raySourceObject.transform.position, -_raySourceObject.transform.up * 20, out _rayObject))
		{
			//Debug.LogError(_rayObject.collider.gameObject.name);
			if(_rayObject.collider.gameObject.name == "Ground")
			{
				GameObject temp = _rayObject.collider.gameObject;
				if(temp.GetComponent<PlatformObject>()._typeOfPlatform == PlatformObject.TypeOfPlatform.UNKNOWN)
				{
					if(_nextMove == NextMove.COUNTNORMAL)
					{
						temp.GetComponent<PlatformObject>()._typeOfPlatform = PlatformObject.TypeOfPlatform.NORMAL;
						_obstaclesPassed += 1;

						if(_obstaclesPassed >= _obstaclesToPass)
						{
							int _randomizer = Random.Range(1,11);
							if(_randomizer > 8)
							{
								_nextMove = NextMove.COUNTNORMAL;
							}
							else if(_randomizer < 8)
							{
								_nextMove = NextMove.SPAWNOBSTACLE;
							}
							else
							{
								if(_turnCounter >= _turnCounterToPass)
								{
									_nextMove = NextMove.TURNANYWHERE;
									PlatformManager.Instance._turnAnyWhere = true;
									return;
								}
							}
							_obstaclesPassed = 0;

							if(_fixedPattern > 10)
							{
								PatternChecker();
							}
							else if(_fixedPattern < 10)
							{
								if(_fixedPattern == 5 || _fixedPattern == 9 || _fixedPattern == 10)
									_obstacleDifficulty = ObstacleDifficulty.HARD;
								else if (_fixedPattern == 3 || _fixedPattern == 7 || _fixedPattern == 8)
									_obstacleDifficulty = ObstacleDifficulty.MEDIUM;
								else
									_obstacleDifficulty = ObstacleDifficulty.EASY;
								RandomizeObstacleToSpawn();
								SetObstaclesToPass();
							}
						}
					}
					else if(_nextMove == NextMove.SPAWNOBSTACLE)
					{
						_fixedPattern ++;
						PlatformObject _platformObj = _rayObject.collider.gameObject.GetComponent<PlatformObject>();
						if( _totalStacks > 0 )
						{
							if(_stackableObstacle == StackableObstacle.HOLED)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.HOLED;
								_platformObj.UpdateThisPlatform();
							}	
							if(_stackableObstacle == StackableObstacle.SPIKED)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.SPIKED;
								_platformObj.UpdateThisPlatform();
								if(_obstacleDirection == ObstacleDirection.FORWARD)
									_platformObj._spikeObject.transform.eulerAngles = new Vector3( 0, 0, 0 );
								if(_obstacleDirection == ObstacleDirection.RIGHT)
									_platformObj._spikeObject.transform.eulerAngles = new Vector3( 0, 90, 0 );
								if(_obstacleDirection == ObstacleDirection.LEFT)
									_platformObj._spikeObject.transform.eulerAngles = new Vector3( 0, -90, 0 );
								if(_obstacleDirection == ObstacleDirection.BACKWARD)
									_platformObj._spikeObject.transform.eulerAngles = new Vector3( 0, 180, 0 );
							}
							if(_stackableObstacle == StackableObstacle.CATCHER)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.CATCHER;
								_platformObj.UpdateThisPlatform();
								if(_totalStacks == _maxStacks)
								{
									_enemyToSpawn = _platformObj._catcherObject.gameObject;
								}
								else if(_totalStacks == 1)
								{
									_enemyToSpawn.GetComponent<TriggerableObjects>()._catcherTrigger.SetActive(true);
									_enemyToSpawn.GetComponent<TriggerableObjects>()._catcherTrigger.gameObject.transform.position = _platformObj._catcherObject.GetComponent<TriggerableObjects>()._catcherObject.transform.position;
									_enemyToSpawn.GetComponent<TriggerableObjects>()._catcherObject.gameObject.transform.LookAt ( _platformObj._catcherObject.GetComponent<TriggerableObjects>()._catcherObject.transform.position );
								}
							}
							if(_stackableObstacle == StackableObstacle.FALLBRIDGE)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.FALLBRIDGE;
								_platformObj.UpdateThisPlatform();
								if(_totalStacks == _maxStacks)
								{
									_fallbridgeToLookat = _platformObj._fallBridgeObject.gameObject;
								}
								if(_totalStacks == 1)
								{
									GameObject tempx = _platformObj._fallBridgeObject.gameObject as GameObject;
									tempx.GetComponent<TriggerableObjects>()._fallBridges[(int)_obstacleDifficulty].SetActive(true);
									tempx.transform.LookAt(_fallbridgeToLookat.transform.position);
									tempx.transform.eulerAngles = new Vector3( 0, tempx.transform.eulerAngles.y ,0 );
								}
							}
							if(_stackableObstacle == StackableObstacle.ENEMY)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.ENEMY;
								_platformObj.UpdateThisPlatform();
								if(_totalStacks == _maxStacks)
								{
									_enemyToSpawn = _platformObj._enemyObject;
									_platformObj._enemyObject.GetComponent<TriggerableObjects>()._path2.SetActive(true);
								}
								if(_totalStacks == _maxStacks-2)
								{
									_enemyToSpawn.GetComponent<TriggerableObjects>()._enemyObject.transform.position = _platformObj._enemyObject.GetComponent<TriggerableObjects>()._enemyObject.transform.position;
									_enemyToSpawn.GetComponent<TriggerableObjects>()._enemyObject.SetActive(true);
								}
								if(_totalStacks == 1)
								{
									_enemyToSpawn.GetComponent<TriggerableObjects>()._path1.transform.position = _platformObj._enemyObject.GetComponent<TriggerableObjects>()._path1.transform.position;
									_enemyToSpawn.GetComponent<TriggerableObjects>()._path1.SetActive(true);
								}
							}		
							_totalStacks --;
						}
						else if( _totalStacks <= 0 )
						{
							if(_stackableObstacle == StackableObstacle.COINED)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.COINED;
								_platformObj.UpdateThisPlatform();
							}
							if(_stackableObstacle == StackableObstacle.SPRING)
							{
								_platformObj._typeOfPlatform = PlatformObject.TypeOfPlatform.SPRING;
								_platformObj.UpdateThisPlatform();
							}
							_nextMove = NextMove.COUNTNORMAL;
							_stackableObstacle = StackableObstacle.NONE;
							_totalStacks = 0;

						}
						_obstaclesPassed = 0;
					}
					_turnCounter++;
				}
			} 
		}
	}

	public void ProgressLevel()
	{
		if(_LevelProgression < 6)
			_LevelProgression++;
		if(_maxObstacles < 8)
			_maxObstacles++;
	}

	void PatternChecker()
	{
		int _randomizer = Random.Range(0,10);
		if(_randomizer < EasyValues[_LevelProgression] ) 
			_obstacleDifficulty = ObstacleDifficulty.EASY;
		if(_randomizer < EasyValues[ _LevelProgression ] + MediumValues[ _LevelProgression ]) 
			_obstacleDifficulty = ObstacleDifficulty.MEDIUM;
		if(_randomizer < EasyValues[ _LevelProgression ] + MediumValues[ _LevelProgression ] + HardValues[ _LevelProgression ] )
			_obstacleDifficulty = ObstacleDifficulty.HARD;
		RandomizeObstacleToSpawn();
		SetObstaclesToPass();
	}

	void RandomizeObstacleToSpawn()
	{
		_stackableObstacle = (StackableObstacle) Random.Range( 1, _maxObstacles );
		if(_stackableObstacle == StackableObstacle.COINED)
		{
			_totalStacks = 0;
		}	
		else if(_stackableObstacle == StackableObstacle.SPRING)
		{
			_totalStacks = 0;
		}
		else if(_stackableObstacle == StackableObstacle.CATCHER)
		{
			_totalStacks = 15;
			_maxStacks = _totalStacks;
		}
		else if(_stackableObstacle == StackableObstacle.HOLED)
		{
			if(_obstacleDifficulty == ObstacleDifficulty.EASY)
				_totalStacks = 2;
			else if(_obstacleDifficulty == ObstacleDifficulty.MEDIUM)
				_totalStacks = 3;
			else if(_obstacleDifficulty == ObstacleDifficulty.HARD)
				_totalStacks = 4;
		}
		else if(_stackableObstacle == StackableObstacle.SPIKED)
		{
			if(_obstacleDifficulty == ObstacleDifficulty.EASY)
				_totalStacks = 1;
			else if(_obstacleDifficulty == ObstacleDifficulty.MEDIUM)
				_totalStacks = 3;
			else if(_obstacleDifficulty == ObstacleDifficulty.HARD)
				_totalStacks = 4;
		}
		else if(_stackableObstacle == StackableObstacle.ENEMY)
		{
			if(_obstacleDifficulty == ObstacleDifficulty.EASY)
				_totalStacks = 10;
			else if(_obstacleDifficulty == ObstacleDifficulty.MEDIUM)
				_totalStacks = 7;
			else if(_obstacleDifficulty == ObstacleDifficulty.HARD)
				_totalStacks = 5;
			_maxStacks = _totalStacks;
		}
		else if(_stackableObstacle == StackableObstacle.FALLBRIDGE)
		{
			if(_obstacleDifficulty == ObstacleDifficulty.EASY)
				_totalStacks = 4;
			else if(_obstacleDifficulty == ObstacleDifficulty.MEDIUM)
				_totalStacks = 6;
			else if(_obstacleDifficulty == ObstacleDifficulty.HARD)
				_totalStacks = 8;
			_maxStacks = _totalStacks;
		}
	}	

	public void SetObstaclesToPass()
	{
		if(_obstacleDifficulty == ObstacleDifficulty.EASY)
		{
			_obstaclesToPass = Random.Range( 7, 9 );
		}
		else if (_obstacleDifficulty == ObstacleDifficulty.MEDIUM)
		{
			_obstaclesToPass = Random.Range( 5, 7 );
		}
		else if(_obstacleDifficulty == ObstacleDifficulty.HARD)
		{
			_obstaclesToPass = Random.Range( 3, 5 );
		}
	}
}
