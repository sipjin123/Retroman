using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformLord: MonoBehaviour {

	#region VARIABLES
	private static PlatformLord _instance;
	public static PlatformLord Instance { get { return _instance; } }

	//ENUMS
	public enum PlatformDirection
	{
		LEFT,
		RIGHT
	}
	public PlatformDirection _platformDirection, _previousPlatformDirection;
	public enum PlatformAction
	{
		SPAWNNORMAL,
		SPAWNTRAP,
		SPAWNTURN,
		SPAWNHEIGHCHANGE,
		SPAWNTRAPPATTERN
	}
	public PlatformAction _platformAction, _previousPlatformAction;
	public enum TrapType
	{
		SPIKE,
		HOLE,
		COINBOX
	}
	public TrapType _trapType;
	public enum DifficultyType
	{
		EASY,
		MEDIUM,
		HARD
	}
	public DifficultyType _difficultyType;


	//LEVEL PROGRESSION
	public int _levelProgression;
	public int _levelProgressionCap;
	public int _totalPlatformsSpawned;

	//DIFFICULTY
	[SerializeField]
	int[] EasyValues, MediumValues, HardValues;

	//OBSTACLE STACKING
	public int _spawnStackCounter;
	public int _spawnStackCapacity;

	//TURNING
	public int _stacksBeforeTurn;
	public int _stacksBeforeTurnCAP;
	int _randomizerChangeDirection;

	//POOLING
	public Transform _platformPool, _platformSpawned;

	public float _bodyRotation;

	//MESH CHANGE
	public int _meshCurrentlySet;
	public int _meshChangeCounter;
	public int _meshChangeCap;

	//VFX
	public int _vfxCounter;
	public int _vfxCap;


	public List<int[]> _patternDict;

	public int [] _pattern1; 
	public int [] _pattern2; 
	public int [] _pattern3; 
	public int [] _pattern4; 
	public int [] _pattern5; 
	public int [] _pattern6; 
	public int [] _pattern7; 
	public int [] _pattern8; 
	public int [] _pattern9; 
	public int [] _pattern10; 

	public int _patternRandomizer;
	#endregion
	//==================================================================================================================================================
	#region INITIALIZATION
	void Awake()
	{
		_instance = this;
	}
	void Start () 
	{
		InitializeVariables();
		for(int i = 0 ; i < 15 ; i++)
		{
			SpawnAPlatform();
		}
	}
	void InitializeVariables()
	{

		//LEVEL PROGRESSION
		_totalPlatformsSpawned = 0;
		_levelProgression = 0;
		_levelProgressionCap = 10;

		//VFX
		_vfxCounter = 0;
		_vfxCap = Random.Range(1,20);
	
		//SPAWNING
		_spawnStackCounter = 10;
		_spawnStackCapacity = 5;

		//TURNING
		_stacksBeforeTurn = 0;
		//_stacksBeforeTurnCAP = Random.Range( 10, 20);
		_stacksBeforeTurnCAP = Random.Range( 50, 250);

		//MESH CHANGING
		_meshCurrentlySet = 0;
		_meshChangeCounter = 0;
		_meshChangeCap = Random.Range ( 0 , 5);


		_bodyRotation = 0;


		_patternDict = new List<int[]>();

		//PATTERNS
		_pattern1 = new int[] {2,2,0,2,2};
		_pattern2 = new int[] {1,2,2,2,1};
		_pattern3 = new int[] {2,0,2,1,1};
		_pattern4 = new int[] {1,1,2,0,2};
		_pattern5 = new int[] {2,2,1,2,2};
		_pattern6 = new int[] {3,1,0,0,0};
		_pattern7 = new int[] {0,1,3,1,3};
		_pattern8 = new int[] {1,6,4,6,0};
		_pattern9 = new int[] {2,6,6,6,2};
		_pattern10 = new int[] {2,4,6,4,2};
		/*
		NORMAL,0
		SPIKED,1
		HOLED,2
		UPSPIKE,3
		LOWHOLE,4
		UPGROUND,5
		LOWGROUND,6
		*/

		_patternDict.Add(_pattern1);
		_patternDict.Add(_pattern2);
		_patternDict.Add(_pattern3);
		_patternDict.Add(_pattern4);
		_patternDict.Add(_pattern5);
		_patternDict.Add(_pattern6);
		_patternDict.Add(_pattern7);
		_patternDict.Add(_pattern8);
		_patternDict.Add(_pattern9);
		_patternDict.Add(_pattern10);

		_patternRandomizer = Random.Range(0 , _patternDict.Count);

		_platformAction = PlatformAction.SPAWNNORMAL;
		//Debug.LogError(_patternDict[0].GetValue(3));
	}
	#endregion
	//==================================================================================================================================================
	#region TESTING
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Z))
		{
			SpawnAPlatform();
		}
		if(Input.GetKey(KeyCode.X))
		{
			SpawnAPlatform();
		}
		if(Input.GetKey(KeyCode.C))
		{
			StartCoroutine(manualSpawnage());
		}
	}
	IEnumerator manualSpawnage()
	{
		yield return new WaitForSeconds (1);
		SpawnAPlatform();
		StartCoroutine(manualSpawnage());
	}
	#endregion
	//==================================================================================================================================================
	#region PRIMARY FUNCTION


	public void SpawnAPlatform()
	{
		//REPLENISH POOL PHASE
		if(_platformPool.childCount < 1 )
		{
			PlatformMinion _pM = _platformSpawned.GetChild(0).GetComponent<PlatformMinion>();
			_pM._typeOfPlatform = PlatformMinion.TypeofPlatform.UNKNOWN;
			_pM.UpdateThisPlatform();
			_pM.transform.parent = _platformPool; 
		}

		//SPAWN PHASE
		GameObject temp = _platformPool.GetChild(0).gameObject;
		temp.SetActive(true);
		temp.transform.position = transform.position;
		temp.transform.rotation = transform.rotation;
		temp.transform.parent = _platformSpawned;

		//VARIABLE DECLARATION PHASSE
		PlatformMinion _platformMinion = temp.GetComponent<PlatformMinion>();


		//APPEARANCE PHASE
		_platformMinion._heightOfPlatform = (PlatformMinion.HeightOfPlatform) _meshCurrentlySet;

		//VFX PHASE
		if(_stacksBeforeTurn < _stacksBeforeTurnCAP - 5 && _stacksBeforeTurn > 5)
		{
			if(Random.Range ( 1 , 5) == 3)
			{
				_vfxCounter ++;
				if( _vfxCounter > _vfxCap )
				{
					_platformMinion._hasTree = true;
					_vfxCap = Random.Range(5,20);
					_vfxCounter = 0;
				}
			}
		}

		//ACTION PHASE
		if(_spawnStackCounter >= 0)
		{
			switch(_platformAction)
			{
				case PlatformAction.SPAWNNORMAL:
					_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.NORMAL;
					break;

				case PlatformAction.SPAWNTRAP:
					if(_trapType == TrapType.SPIKE)
					{
						_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.SPIKED;
					}
					else if(_trapType == TrapType.HOLE)
					{
						_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.HOLED;
					}
					else if(_trapType == TrapType.COINBOX)
					{
						_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.COINBOX;
					}
					break;
				case PlatformAction.SPAWNHEIGHCHANGE:
					_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.NORMAL;
					break;
				case PlatformAction.SPAWNTRAPPATTERN:
						_platformMinion._typeOfPlatform = (PlatformMinion.TypeofPlatform)  _patternDict[_patternRandomizer].GetValue(_spawnStackCounter) ;
						_platformMinion.transform.eulerAngles = new Vector3(0,45,0);
						break;
			}
		}
		else if (_spawnStackCounter < 0 )
		{
			_platformMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.NORMAL;
			SetupNextAction(temp, _platformMinion);
		}
		_stacksBeforeTurn ++;
		_totalPlatformsSpawned ++;
		_spawnStackCounter --;


		_platformMinion.UpdateThisPlatform();
		temp = null;
		transform.position += transform.forward * 1f;
	}
	#endregion
	private void SetupNextAction(GameObject _temp, PlatformMinion _platMinion)
	{
		_previousPlatformAction = _platformAction;



		if(_totalPlatformsSpawned > _levelProgressionCap)
		{
			_levelProgressionCap = (int) (_levelProgressionCap * 1.5f);
			if(_levelProgression < 6)
				_levelProgression++;

		}







		if(_previousPlatformAction == PlatformAction.SPAWNHEIGHCHANGE)
		{
			SetAppearance();
			_platformAction = PlatformAction.SPAWNNORMAL;
		}
		else if(_previousPlatformAction == PlatformAction.SPAWNTRAP)
		{
			_platformAction = PlatformAction.SPAWNHEIGHCHANGE;
		}
		else if(_previousPlatformAction == PlatformAction.SPAWNTRAPPATTERN)
		{
			_platformAction = PlatformAction.SPAWNNORMAL;
		}
		else if(_previousPlatformAction == PlatformAction.SPAWNTURN)
		{
			_platformAction = PlatformAction.SPAWNNORMAL;
		}
		else if(_previousPlatformAction == PlatformAction.SPAWNNORMAL)
		{
			//TURNING CONDITION
			if(_stacksBeforeTurn > _stacksBeforeTurnCAP)
			{
				if(Random.Range ( 0, 4) == 3)
				{
					_platformAction = PlatformAction.SPAWNTURN;
					SwitchDirection(_temp, _platMinion);
				}
			}
			//TRAP CONDITION
			else
			{
				_trapType = (TrapType)Random.Range(0,3);
				_platformAction = PlatformAction.SPAWNTRAP;
			}
		}

		SetDifficulty();
		SetStackableObstacles();
	}
	//==================================================================================================================================================
	#region SWITCHDIRECTION
	private void SwitchDirection(GameObject _temp, PlatformMinion _platMinion)
	{
		_previousPlatformDirection = _platformDirection;
		if(_previousPlatformDirection == PlatformDirection.LEFT)
			_platformDirection = PlatformDirection.RIGHT;
		else if(_previousPlatformDirection == PlatformDirection.RIGHT)
			_platformDirection = PlatformDirection.LEFT;

		switch(_platformDirection)
		{
		case PlatformDirection.LEFT:
			_bodyRotation -= 90;
			_platMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.LEFT;
			break;
		case PlatformDirection.RIGHT:
			_bodyRotation += 90;
			_platMinion._typeOfPlatform = PlatformMinion.TypeofPlatform.RIGHT;
			break;
		}
		_platMinion.UpdateThisPlatform();
		transform.rotation = Quaternion.Euler( new Vector3(0, _bodyRotation, 0) );
		_stacksBeforeTurn = 0;
		_stacksBeforeTurnCAP = Random.Range( 150, 250);
		SetupNextAction(_temp, _platMinion);
	}
	#endregion

	//==================================================================================================================================================
	#region DIFFICULTY LEVEL
	private void SetDifficulty()
	{
		int randomizer = Random.Range(0,10);
		if(randomizer < EasyValues[_levelProgression] ) 
		{
			_difficultyType = DifficultyType.EASY;
		}
		else if(randomizer < EasyValues[ _levelProgression ] + MediumValues[ _levelProgression ]) 
		{
			_difficultyType = DifficultyType.MEDIUM;
		}
		else if(randomizer < EasyValues[ _levelProgression ] + MediumValues[ _levelProgression ] + HardValues[ _levelProgression ] )
		{
			_difficultyType = DifficultyType.HARD;
		}
	}
	#endregion

	//==================================================================================================================================================
	#region SET STACKS
	private void SetStackableObstacles()
	{
		if(_difficultyType == DifficultyType.EASY)
		{
			if( _platformAction == PlatformAction.SPAWNHEIGHCHANGE )
				_spawnStackCounter = Random.Range( 3, 6 );
			else if( _platformAction == PlatformAction.SPAWNNORMAL )
				_spawnStackCounter = Random.Range( 7, 9 );
			else if( _platformAction == PlatformAction.SPAWNTRAP )
			{
				switch(_trapType)
				{
					case TrapType.SPIKE:
						_spawnStackCounter = 1;
						break;
					case TrapType.HOLE:
						_spawnStackCounter = 3;
						break;
					case TrapType.COINBOX:
						_spawnStackCounter = 1;
						break;
				}
			}
		}
		else if(_difficultyType == DifficultyType.MEDIUM)
		{
			if( _platformAction == PlatformAction.SPAWNHEIGHCHANGE )
				_spawnStackCounter = Random.Range( 2, 5 );
			else if( _platformAction == PlatformAction.SPAWNNORMAL )
				_spawnStackCounter = Random.Range( 5, 7 );
			else if( _platformAction == PlatformAction.SPAWNTRAP )
			{
				switch(_trapType)
				{
					case TrapType.SPIKE:
						_spawnStackCounter = 3;
						break;
					case TrapType.HOLE:
						_spawnStackCounter = 4;
						break;
					case TrapType.COINBOX:
						_spawnStackCounter = 1;
						break;
				}
			}
		}
		else if(_difficultyType == DifficultyType.HARD)
		{
			
			if(_platformAction == PlatformAction.SPAWNTRAP)
			{
				int _imbalanceRandomizer = Random.Range(0,3);
				if(_imbalanceRandomizer == 2)
				{
					_platformAction = PlatformAction.SPAWNTRAPPATTERN;
					_spawnStackCapacity = 5;
					_spawnStackCounter = 5;
					_patternRandomizer = Random.Range(0 , _patternDict.Count);
					return;
				}
			}


			if( _platformAction == PlatformAction.SPAWNHEIGHCHANGE )
				_spawnStackCounter = Random.Range( 2, 4 );
			else if( _platformAction == PlatformAction.SPAWNNORMAL )
				_spawnStackCounter = Random.Range( 3, 5 );
			else if( _platformAction == PlatformAction.SPAWNTRAP )
			{
				switch(_trapType)
				{
					case TrapType.SPIKE:
						_spawnStackCounter = 4;
						break;
					case TrapType.HOLE:
						_spawnStackCounter = 5;
						break;
					case TrapType.COINBOX:
						_spawnStackCounter = 1;
						break;
				}
				_spawnStackCapacity = 5;
			}
		}
	}
	#endregion

	//==================================================================================================================================================
	#region HEIGH OF PLATFORM
	private void SetAppearance()
	{
		_meshChangeCounter ++;
		if(_meshChangeCounter > _meshChangeCap)
		{
			if(_meshCurrentlySet == 0)
			{
				_meshCurrentlySet = 1;
			}
			else if(_meshCurrentlySet == 1)
			{
				int _randomizer = Random.Range(0,2);
				if(_randomizer == 0)
					_meshCurrentlySet = 0;
				else if(_randomizer == 1)
					_meshCurrentlySet = 2;
			}
			else if(_meshCurrentlySet == 2)
			{
				int _randomizer = Random.Range(0,2);
				if(_randomizer == 0)
					_meshCurrentlySet = Random.Range(0,2);
				else if(_randomizer == 1)
					_meshCurrentlySet = 3;
			}
			else if(_meshCurrentlySet == 3)
			{
				_meshCurrentlySet = Random.Range(0,3);
			}
			_meshChangeCap = 10;
			//_meshChangeCap = Random.Range (9 , 35);
		}
	}
	#endregion



	void OnGUIx()
	{
		GUI.Box(new Rect(0,Screen.height-30,300,30),""+_platformAction+" "+_difficultyType+" "+_levelProgression);
	}
}
