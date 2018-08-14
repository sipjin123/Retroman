using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Utils;


using Retroman;
using UniRx;

public class GameControls : MonoBehaviour
    {
        #region VARIABLES

        public enum GameState
        {
            START,
            GAMEOVER
        }
        public GameState _gameState;

        //SCORING
        private float Score;
        public Text UIScore;
        public Text UIScore2;
        public Text UIHighScore;
        public Text UIHighScore2;

        //UI WINDOWS
        public GameObject InGameWindow;
        public GameObject _resultCharParent;
        public GameObject _pauseButton, _resumeButton;

    public UnityEngine.UI.Button ResetButton;
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
            Factory.Get<DataManagerService>().MessageBroker.Receive<TogglePause>().Subscribe(_ =>
            {
                if (_isPaused)
                {
                    LevelPause(false);
                }
                else
                {
                    LevelPause(true);
                }
            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<GameOver>().Subscribe(_ =>
            {
                if (Factory.Get<DataManagerService>().IFTestMode)
                {
                    return;
                }
                GameOverIT();
            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_=>
            {
                StartButton();
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<UpdateScore>().Subscribe(_ => 
            {

                UptateScoreing();
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<AddScore>().Subscribe(_ =>
            {
                Score += _.ScoreToAdd;
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
    
        //==========================================================================================================================================
        void UptateScoreing()
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
            StartCoroutine(DelayStart());
        }
        public IEnumerator DelayStart()
        {
            _camAnim.enabled = true;
            _playerAnim.enabled = true;
            yield return new WaitForSeconds(2.55f);

            Debug.LogError("game controls :: delayed start");
            _camAnim.enabled = false;
            _playerAnim.enabled = false;

            Factory.Get<DataManagerService>().MessageBroker.Publish(new PauseGame { IfPause = false });
        InGameWindow.SetActive(true);

            UIScore.text = "0";
            UIScore2.text = "0";
            UIHighScore.text = "Best " + PlayerPrefs.GetInt("hiSkor", 0);
            UIHighScore2.text = "Best " + PlayerPrefs.GetInt("hiSkor", 0);

            Debug.LogError("game controls :: Updated UI Stuff");
            StartCoroutine(TimeBombStartDesign());
        }
        //==========================================================================================================================================
        #region IN GAME BUTTONS
        public void LevelPause(bool _switch)
        {
        _isPaused = _switch;


            SoundControls.Instance._buttonClick.Play();
            if (_switch)
            {
                SoundControls.Instance.PauseBGM();
                Factory.Get<DataManagerService>().MessageBroker.Publish(new ActivePlayerObject { IfActive = false });
                _resumeButton.SetActive(true);
                _pauseButton.SetActive(false);
                _storeTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
        {
            SoundControls.Instance.REsumeBGM();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ActivePlayerObject { IfActive = true });
                _resumeButton.SetActive(false);
                _pauseButton.SetActive(true);
                Time.timeScale = _storeTimeScale;
            }
        }
        public void ResetGame()
        {
        ResetButton.interactable = false;
            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = Framework.EScene.GameRoot });
        }
        #endregion
        //==========================================================================================================================================
        #region GAME OVER
        void GameOverIT()
        {
            _gameState = GameState.GAMEOVER;
            if (_gameState == GameState.GAMEOVER)
            {
                if (!ifGameOverplayed)
                {
                //  Factory.Get<DataManagerService>().PlayerControls._deathAnim.SetActive(true);
                Factory.Get<DataManagerService>().MessageBroker.Publish(new EnableRagdoll());
                StartCoroutine(GameOverDelay());
                    ifGameOverplayed = true;
                }
                SoundControls.Instance._sfxBGM.gameObject.SetActive(false);
                // Factory.Get<DataManagerService>().PlayerControls.enabled = false;

                Factory.Get<DataManagerService>().MessageBroker.Publish(new EnablePlayerControls { IfACtive = false});
        }
        }
        public IEnumerator GameOverDelay()
        {
            InGameWindow.SetActive(false);
            //Factory.Get<DataManagerService>().PlayerControls.transform.GetChild(0).gameObject.SetActive(false);
        
            Factory.Get<DataManagerService>().MessageBroker.Publish(new DisablePlayableCharacter());
            //Factory.Get<DataManagerService>().PlayerControls._shadowObject.SetActive(false);

            Factory.Get<DataManagerService>().MessageBroker.Publish(new EnablePlayerShadows { IfActive = false });


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
            /*
            _resultCharParent.SetActive(true);
            _resultCharParent.transform.GetChild(PlayerPrefs.GetInt("CurrentCharacter", 0) + 1).gameObject.SetActive(true);
            */
            Time.timeScale = 0;

            Factory.Get<DataManagerService>().MessageBroker.Publish(new EndGame ());

        Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = true });


    }
        #endregion
    }