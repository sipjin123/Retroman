using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Utils;


using Retroman;
using UniRx;

public class GameControls : MonoBehaviour
    {
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
            Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_=> 
            {
                StartButton();
            }).AddTo(this);

        }
        void Start()
        {
            _isPaused = false;
            _gameState = GameState.START;
            Score = 0;
        }
        public IEnumerator TimeBombStartDesign()
        {
            yield return new WaitForSeconds(5);
            Destroy(_startUpDesign);
        }
        #endregion
        //==========================================================================================================================================
        #region SIGNALS
            /*
        void OnEnable()
        {

            S88Signals.ON_GAME_START.AddListener(OnGameSTartup);
            S88Signals.ON_GAME_PAUSE.AddListener(OnGamePause);
            S88Signals.ON_GAME_RESUME.AddListener(OnGameResume);

            try {
                SoundControls.Instance._sfxBGM.gameObject.SetActive(true);
            }
            catch { }
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
        }*/
        #endregion
        //==========================================================================================================================================
        public void UptateScoreing()
        {
            UIScore.text = "" + (int)Score;
            UIScore2.text = "" + (int)Score;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                StartButton();
            }
            if (Input.GetKeyDown(KeyCode.Q))
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

        Factory.Get<DataManagerService>().MessageBroker.Publish(new PauseGame { IfPause = false });
            //S88Signals.ON_GAME_RESUME.Dispatch();
            InGameWindow.SetActive(true);

            UIScore.text = "0";
            UIScore2.text = "0";
            UIHighScore.text = "Best " + PlayerPrefs.GetInt("hiSkor", 0);
            UIHighScore2.text = "Best " + PlayerPrefs.GetInt("hiSkor", 0);
            StartCoroutine(TimeBombStartDesign());
        }
        //==========================================================================================================================================
        #region IN GAME BUTTONS
        public void LevelPause(bool _switch)
        {
            if (_switch)
            {
                PlayerControls.Instance._activePlayerObject = false;
                _resumeButton.SetActive(true);
                _pauseButton.SetActive(false);
                _storeTimeScale = Time.timeScale;
                Time.timeScale = 0;
               // S88Signals.ON_GAME_PAUSE.Dispatch();
            }
            else
            {
                PlayerControls.Instance._activePlayerObject = true;
                _resumeButton.SetActive(false);
                _pauseButton.SetActive(true);
                Time.timeScale = _storeTimeScale;
             //   S88Signals.ON_GAME_RESUME.Dispatch();
            }
        }
        public void ResetGame()
        {

            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = Framework.EScene.GameRoot });
        }
        #endregion
        //==========================================================================================================================================
        #region GAME OVER
        public void GameOverIT()
        {
            _gameState = GameState.GAMEOVER;
            if (_gameState == GameState.GAMEOVER)
            {
                if (!ifGameOverplayed)
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

            Factory.Get<DataManagerService>().MessageBroker.Publish(new AddCoin { CoinsToAdd = (int)Score });

            //SET SCORE
            PlayerPrefs.SetInt("curSkor", (int)Score);

            //SET HIGH SCORE
            if (Score > PlayerPrefs.GetInt("hiSkor", 0))
                PlayerPrefs.SetInt("hiSkor", (int)Score);
            UIHighScore.GetComponent<Text>().text = "HS: " + PlayerPrefs.GetInt("hiSkor", 0);
            UIHighScore2.GetComponent<Text>().text = "HS: " + PlayerPrefs.GetInt("hiSkor", 0);

            //SOUND EFFECTS
            SoundControls.Instance._sfxDie.Play();


            //SET DELAY
            yield return new WaitForSeconds(2);
            _resultCharParent.SetActive(true);
            _resultCharParent.transform.GetChild(PlayerPrefs.GetInt("CurrentCharacter", 0) + 1).gameObject.SetActive(true);

            Time.timeScale = 0;

            Factory.Get<DataManagerService>().MessageBroker.Publish(new EndGame ());
          
        }
        #endregion
    }