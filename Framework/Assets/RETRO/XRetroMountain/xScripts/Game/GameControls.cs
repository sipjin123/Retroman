using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Utils;
using Retroman;
using UniRx;
using Sirenix.OdinInspector;
using Sandbox.Popup;
using Framework;
using System;

public class GameControls : SerializedMonoBehaviour
{
    #region VARIABLES


    //SCORING
    private float Score;
    [FoldoutGroup("Scoring"), SerializeField]
    private Text
        UIScore,
        UIScore2,
        UIHighScore,
        UIHighScore2;

    [FoldoutGroup("Buttons"), SerializeField]
    private GameObject _pauseButton, _resumeButton;
    [FoldoutGroup("Buttons"), SerializeField]
    private UnityEngine.UI.Button ResetButton;




    //UI WINDOWS
    public Canvas _InGameWindow;
    public bool CanAccessBackButton()
    {
        if (_InGameWindow.enabled)
            return true;
        return false;
    }

    //INTRO ANIMATIONS
    public ManualAnimationScene _camAnim, _playerAnim;

    //IN GAME OPTIONS
    public bool _isPaused;
    MessageBroker _Broker;
    public GameObject _startUpDesign;

    float _storeTimeScale;
    #endregion
    //==========================================================================================================================================
    #region INITIALIZATION
    void Awake()
    {
        _Broker = Factory.Get<DataManagerService>().MessageBroker;



        if (Factory.Get<DataManagerService>().GetHighScore() == 0)
        {
            UIHighScore.enabled = false;
            UIHighScore2.enabled = false;
        }
        _Broker.Receive<TogglePause>().Subscribe(_ =>
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
        _Broker.Receive<GameOverSignal>().Subscribe(_ =>
        {
            if (Factory.Get<DataManagerService>().IFTestMode)
            {
                //return;
            }
            GameOverIT(_.KilledBy);
        }).AddTo(this);
        _Broker.Receive<LaunchGamePlay>().Subscribe(_ =>
        {
            StartButton();
        }).AddTo(this);

        _Broker.Receive<UpdateScore>().Subscribe(_ =>
        {

            UptateScoreing();
        }).AddTo(this);

        _Broker.Receive<AddScore>().Subscribe(_ =>
        {
            Score += _.ScoreToAdd;
        }).AddTo(this);
    }
    void Start()
    {
        _isPaused = false;
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

        _Broker.Publish(new PauseGame { IfPause = false });
        _InGameWindow.enabled = (true);

        UIScore.text = "0";
        UIScore2.text = "0";
        UIHighScore.text = "Best " + Factory.Get<DataManagerService>().GetHighScore();
        UIHighScore2.text = "Best " + Factory.Get<DataManagerService>().GetHighScore();

        StartCoroutine(TimeBombStartDesign());

        _Broker.Publish(new RegisterStats { TypeOfStatisticData = TypeOfStatisticData.InitRun });
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
            _Broker.Publish(new ActivePlayerObject { IfActive = false });
            _resumeButton.SetActive(true);
            _pauseButton.SetActive(false);
            _storeTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            SoundControls.Instance.REsumeBGM();
            _Broker.Publish(new ActivePlayerObject { IfActive = true });
            _resumeButton.SetActive(false);
            _pauseButton.SetActive(true);
            Time.timeScale = _storeTimeScale;
        }
    }
    public void ResetGame()
    {
        Debug.LogError("Reached Reset Button Feasture");
        ResetButton.interactable = false;
        SoundControls.Instance._buttonClick.Play();
        _Broker.Publish(new ChangeScene { Scene = Framework.EScene.GameRoot });
    }
    #endregion
    //==========================================================================================================================================
    #region GAME OVER
    string cachedPlatformType;
    void GameOverIT(string platformType)
    {
        cachedPlatformType = platformType;
        //  Factory.Get<DataManagerService>().PlayerControls._deathAnim.SetActive(true);
        _Broker.Publish(new EnableRagdoll());
        StartCoroutine(GameOverDelay());

        SoundControls.Instance._sfxBGM.gameObject.SetActive(false);
        // Factory.Get<DataManagerService>().PlayerControls.enabled = false;

        _Broker.Publish(new EnablePlayerControls { IfACtive = false });
    }
    public IEnumerator GameOverDelay()
    {
        _InGameWindow.enabled = (false);

        _Broker.Publish(new DisablePlayableCharacter());
        _Broker.Publish(new EnablePlayerShadows { IfActive = false });
        _Broker.Publish(new AddCoin { CoinsToAdd = (int)Score });

        //SET SCORE
        Factory.Get<DataManagerService>().SetScore(Score);
        if (Score > Factory.Get<DataManagerService>().GetHighScore())
        {
            Factory.Get<DataManagerService>().SetHighScore(Score);
        }

        UIHighScore.GetComponent<Text>().text = "HS: " + Factory.Get<DataManagerService>().GetHighScore();
        UIHighScore2.GetComponent<Text>().text = "HS: " + Factory.Get<DataManagerService>().GetHighScore();

        //SOUND EFFECTS
        SoundControls.Instance._sfxDie.Play();

        //SET DELAY
        yield return new WaitForSeconds(2);
        Time.timeScale = 0;

        Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Show(PopupType.ResultsPopup);
        _Broker.Publish(new RegisterStats
        {
            TypeOfStatisticData = TypeOfStatisticData.ResultsData,
            RawData = new ProcessResults
            {
                TotalScore = Score,
                PlatformDeath = cachedPlatformType
            }
        });
        _Broker.Publish(new AutomatedUIState
        {
            Scene = EScene.ResultRoot
        });

        _Broker.Publish(new EndGame());
        _Broker.Publish(new ToggleCoins { IfActive = true });
    }
    #endregion
}