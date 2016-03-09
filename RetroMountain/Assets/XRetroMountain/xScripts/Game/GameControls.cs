using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Synergy88;
using Common.Signal;


public class GameControls : MonoBehaviour {
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
	public GameObject UITExt;
	public GameObject UITExt2;
	public GameObject UIHS;
	public GameObject UIHS2;

	//UI WINDOWS
	public GameObject InGameWindow;
	public GameObject _resultCharParent;
	public GameObject _pauseButton, _resumeButton;

	//INTRO ANIMATIONS
	public ManualAnimationScene _camAnim, _playerAnim;

	//IN GAME OPTIONS
	bool ifGameOverplayed;
	public bool _isPaused;
	//==========================================================================================================================================
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
	//==========================================================================================================================================
	//SIGNALS
	void OnEnable()
	{
		S88Signals.ON_GAME_START.AddListener(OnGameSTartup);
		S88Signals.ON_GAME_PAUSE.AddListener(OnGamePause);
		S88Signals.ON_GAME_RESUME.AddListener(OnGameResume);
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
	//==========================================================================================================================================
	public void UptateScoreing()
	{
		UITExt.GetComponent<Text>().text = ""+(int)Score;
		UITExt2.GetComponent<Text>().text = ""+(int)Score;
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


		UIHS.GetComponent<Text>().text = ""+PlayerPrefs.GetInt("hiSkor",0);
		UIHS2.GetComponent<Text>().text = ""+PlayerPrefs.GetInt("hiSkor",0);
	}
	//==========================================================================================================================================
	public void LevelPause( bool _switch)
	{
		if(_switch)
		{
			PlayerControls.Instance._activePlayerObject = false;
			_resumeButton.SetActive(true);
			_pauseButton.SetActive(false);
			Time.timeScale = 0;
			S88Signals.ON_GAME_PAUSE.Dispatch();
		}
		else
		{
			PlayerControls.Instance._activePlayerObject = true;
			_resumeButton.SetActive(false);
			_pauseButton.SetActive(true);
			Time.timeScale = 1.5f;
			S88Signals.ON_GAME_RESUME.Dispatch();
		}
	}
	public void ResetGame()
	{
		S88Scene.Load<GameRoot>(EScene.Game);
		S88Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);
		S88Scene.Load<HomeRoot>(EScene.Home);
	}
	public void ShowADS()
	{
		S88Signals.ON_SHOW_UNITY_ADS.Dispatch();
	}
	//==========================================================================================================================================
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
			SoundControls.Instance._sfxBGM.mute = true;
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
		UIHS.GetComponent<Text>().text = "HS: "+PlayerPrefs.GetInt("hiSkor",0);
		UIHS2.GetComponent<Text>().text = "HS: "+PlayerPrefs.GetInt("hiSkor",0);

		//SOUND EFFECTS
		SoundControls.Instance._sfxDie.Play();


		//SET DELAY
		yield return new WaitForSeconds(2);

		_resultCharParent.transform.GetChild( PlayerPrefs.GetInt("CurrentCharacter",0) ).gameObject.SetActive(true);

		S88Scene.Load<ResultsRoot>(EScene.Results);
		S88Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);
		S88Signals.ON_GAME_OVER.Dispatch();

		//SET ADS
		PlayerPrefs.SetInt("AdCounter",  PlayerPrefs.GetInt("AdCounter",0) + 1  );
		if( PlayerPrefs.GetInt("AdCounter",0) >= 3)
		{
			ShowADS();
			PlayerPrefs.SetInt("AdCounter", 0);
		}
	}

}
