using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameControls : MonoBehaviour {
	private static GameControls _instance;
	public static GameControls Instance { get { return _instance; } }

	public enum GameState
	{
		START,
		GAMEOVER
	}
	public GameState _gameState;
	public float Score;
	public GameObject UITExt;
	public GameObject UIHS;
	public GameObject UILVLProgtxt;
	public GameObject UILVLProgButton;
	public GameObject StartButtonx;
	public GameObject loseIndic;
	public GameObject uiplatpassed;
	void Awake()
	{
		_instance = this;
	}
	public void LvlprogressBuytton()
	{
		ObstacleManager.Instance.ProgressLevel();
	}
	public void StartButton()
	{
		PlayerControls.Instance._activePlayerObject = true;
		StartButtonx.SetActive(false);
	}
	void Start () 
	{
		_gameState = GameState.START;
		Score = 0;
	}
	bool ifGameOverplayed;
	void Update () 
	{
		UITExt.GetComponent<Text>().text = "CS: "+(int)Score;
		UIHS.GetComponent<Text>().text = "HS: "+PlayerPrefs.GetInt("hiSkor",0);
		UILVLProgtxt.GetComponent<Text>().text = "Level Progress: "+ObstacleManager.Instance._LevelProgression;
		uiplatpassed.GetComponent<Text>().text = "Platforms Spawned: "+PlatformManager.Instance._TOTALSPAWNEDPLATFORMS;

		if(!PlayerControls.Instance._activePlayerObject)
		{
			if(Input.GetKeyDown(KeyCode.Space))
				StartButton();
			return;
		}
		if(_gameState != GameState.GAMEOVER)
		{
			//Score += 1* 0.25f;
		}
		else
		{
			loseIndic.SetActive(true);
			if(!ifGameOverplayed)
			{
				if(Score > PlayerPrefs.GetInt("hiSkor",0))
					PlayerPrefs.SetInt("hiSkor",(int)Score);
				SoundControls.Instance._sfxDie.Play();
				ifGameOverplayed = true;
			}
			SoundControls.Instance._sfxBGM.mute = true;
			PlayerControls.Instance.gameObject.GetComponent<PlayerControls>().enabled = false;
			if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
			{
				Application.LoadLevel("Game");
			}
		}
	}
}
