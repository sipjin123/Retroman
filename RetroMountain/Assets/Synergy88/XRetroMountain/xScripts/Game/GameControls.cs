using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Synergy88;
using Common.Signal;


public class GameControls : MonoBehaviour {
	#region VARIABLES
	private static GameControls _instance;
	public static GameControls Instance { get { return _instance; } }

	public enum GameState
	{
		START,
		GAMEOVER
	}
	public GameState _gameState;

	//SCORING
	public float Score;
	public Text UIScore;
	public Text UIScore2;
	public Text UIHighScore;
	public Text UIHighScore2;

	//UI WINDOWS
	public GameObject InGameWindow;
	public GameObject _resultCharParent;
	public GameObject _pauseButton, _resumeButton;

	//INTRO ANIMATIONS
	public ManualAnimationScene _camAnim, _playerAnim;

	//IN GAME OPTIONS
	bool ifGameOverplayed;
	public bool _isPaused;

	public GameObject _startUpDesign;

	float _storeTimeScale;
	#endregion
	//==========================================================================================================================================
	#region INITIALIZATION
	void Awake()
	{
		_instance = this;
	}
	void Start () 
	{
		_isPaused = false;
		_gameState = GameState.START;
		Score = 0;
	}
	public IEnumerator TimeBombStartDesign()
	{
		yield return new WaitForSeconds( 5 );
		Destroy(_startUpDesign);
	}
	#endregion
	//==========================================================================================================================================
	#region SIGNALS
	void OnEnable()
	{
		S88Signals.ON_GAME_START.AddListener(OnGameSTartup);
		S88Signals.ON_GAME_PAUSE.AddListener(OnGamePause);
		S88Signals.ON_GAME_RESUME.AddListener(OnGameResume);

		try{
		SoundControls.Instance._sfxBGM.gameObject.SetActive(true);
		}
		catch{}
	}
	void OnDisable()
	{
		S88Signals.ON_GAME_START.RemoveListener(OnGameSTartup);
		S88Signals.ON_GAME_PAUSE.RemoveListener(OnGamePause);
		S88Signals.ON_GAME_RESUME.RemoveListener(OnGameResume);
	}
	void OnGamePause(ISignalParameters parameters)
	{
		_isPaused = true;
	}
	void OnGameResume(ISignalParameters parameters)
	{
		_isPaused = false;
	}
	private void OnGameSTartup(ISignalParameters parameters) {
		StartButton();
	}
	#endregion
	//==========================================================================================================================================
	public void UptateScoreing()
	{
		UIScore.text = ""+(int)Score;
		UIScore2.text = ""+(int)Score;
		S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddParameter(S88Params.PLAYER_CURRENCY, 1);
		S88Signals.ON_UPDATE_PLAYER_CURRENCY.Dispatch();
	}
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			StartButton();	
		}
		if(Input.GetKeyDown(KeyCode.Q))
		{
			ResetGame();
		}
	}
	//==========================================================================================================================================
	public void StartButton()
	{
		_camAnim.enabled = true;
		_playerAnim.enabled = true;
		StartCoroutine(DelayStart());
	}
	public IEnumerator DelayStart()
	{
		yield return new WaitForSeconds(2.55f);

		_camAnim.enabled = false;
		_playerAnim.enabled = false;

		S88Signals.ON_GAME_RESUME.Dispatch();
		InGameWindow.SetActive(true);

		UIScore.text = "0";
		UIScore2.text = "0";
		UIHighScore.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);
		UIHighScore2.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);
		StartCoroutine(TimeBombStartDesign());
	}
	//==========================================================================================================================================
	#region IN GAME BUTTONS
	public void LevelPause( bool _switch)
	{
		if(_switch)
		{
			PlayerControls.Instance._activePlayerObject = false;
			_resumeButton.SetActive(true);
			_pauseButton.SetActive(false);
			_storeTimeScale = Time.timeScale;
			Time.timeScale = 0;
			S88Signals.ON_GAME_PAUSE.Dispatch();
		}
		else
		{
			PlayerControls.Instance._activePlayerObject = true;
			_resumeButton.SetActive(false);
			_pauseButton.SetActive(true);
			Time.timeScale = _storeTimeScale;
			S88Signals.ON_GAME_RESUME.Dispatch();
		}
	}
	public void ResetGame()
	{
		S88Scene.Load<HomeRoot>(EScene.Home);
		S88Scene.Load<GameRoot>(EScene.Game);
		S88Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);
	}
	public void ShowADS()
	{
		S88Signals.ON_SHOW_UNITY_ADS.Dispatch();
	}
	#endregion
	//==========================================================================================================================================
	#region GAME OVER
	public void GameOverIT()
	{
		_gameState = GameState.GAMEOVER;
		if(_gameState == GameState.GAMEOVER)
		{
			if(!ifGameOverplayed)
			{
				PlayerControls.Instance._deathAnim.SetActive(true);
				StartCoroutine(GameOverDelay());
				ifGameOverplayed = true;
			}
			SoundControls.Instance._sfxBGM.gameObject.SetActive(false);
			PlayerControls.Instance.gameObject.GetComponent<PlayerControls>().enabled = false;
		}
	}
	public IEnumerator GameOverDelay()
	{
		InGameWindow.SetActive(false);
		PlayerControls.Instance.transform.GetChild(0).gameObject.SetActive(false);
		PlayerControls.Instance._shadowObject.SetActive(false);


		//SET HIGH GOLD
		S88Signals.ON_UPDATE_PLAYER_CURRENCY.ClearParameters();
		S88Signals.ON_UPDATE_PLAYER_CURRENCY.AddParameter(S88Params.PLAYER_CURRENCY,(int)Score);
		S88Signals.ON_UPDATE_PLAYER_CURRENCY.Dispatch();

		//SET SCORE
			PlayerPrefs.SetInt("curSkor",(int)Score);

		//SET HIGH SCORE
		if(Score > PlayerPrefs.GetInt("hiSkor",0))
			PlayerPrefs.SetInt("hiSkor",(int)Score);
		UIHighScore.GetComponent<Text>().text = "HS: "+PlayerPrefs.GetInt("hiSkor",0);
		UIHighScore2.GetComponent<Text>().text = "HS: "+PlayerPrefs.GetInt("hiSkor",0);

		//SOUND EFFECTS
		SoundControls.Instance._sfxDie.Play();


		//SET DELAY
		yield return new WaitForSeconds(2);
		_resultCharParent.SetActive(true);
		_resultCharParent.transform.GetChild( PlayerPrefs.GetInt("CurrentCharacter",0)+1 ).gameObject.SetActive(true);

		Time.timeScale = 0;

		S88Scene.Load<ResultsRoot>(EScene.Results);
		S88Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);
		S88Signals.ON_GAME_OVER.Dispatch();

		//SET ADS
		PlayerPrefs.SetInt("AdCounter",  PlayerPrefs.GetInt("AdCounter",0) + 1  );
		if( PlayerPrefs.GetInt("AdCounter",0) >= 3)
		{
			//ShowADS();
			S88Signals.ON_SHOW_INTERSTITIAL_ADS.Dispatch();
			PlayerPrefs.SetInt("AdCounter", 0);
		}
	}
	#endregion
}
