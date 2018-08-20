using UnityEngine;
using System.Collections;

using Common.Signal;
using Framework;
using Common.Utils;
using UniRx;
using Common.Query;

namespace Retroman {
	
	public class SettingsRoot : Scene
    {

		public GameObject _toggleBGM, _toggleSFX;
		bool _BGMswitch, _SFXswitch;
		public GameObject _CreditsWindow;

        public CanvasGroup InteractiveCanvas;
        bool _DisableBackButton;

        protected override void Awake()
        {
			if(PlayerPrefs.GetInt("BGMSWITCH",1) == 0)
				_BGMswitch = false;
			else
				_BGMswitch = true;
			if(PlayerPrefs.GetInt("SFXSWITCH",1) == 0)
				_SFXswitch = false;
			else
				_SFXswitch = true;

			_toggleSFX.SetActive( _SFXswitch );
			_toggleBGM.SetActive( _BGMswitch );


             base.Awake();
            SetupSignals();
            SetupButtons();
		}

        void SetupButtons()
        {
            this.AddButtonHandler(EButton.Back, (ButtonClickedSignal signal) =>
            {
                BackButtonClick();
            });
        }
        void SetupSignals()
        {
            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                Debug.LogError(D.B + " SettingsRoot :: Received Soft Back Button");
                Debug.LogError(D.B + " SettingsRoot :: Received Soft Back Button" + _.BackButtonType);
                if (_.BackButtonType == BackButtonType.SceneIsSettings)
                {
                    Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });

                    BackButtonClick();
                }
            });
        }

        public void CreditsOnClick()
        {
            _CreditsWindow.SetActive(true);
            SoundControls.Instance._buttonClick.Play();
        }

        protected override void OnEnable() {
			base.OnEnable();
			_CreditsWindow.SetActive(false);
		}
        
		public void SwitchBGM()
		{
			SoundControls.Instance._buttonClick.Play();
			if(_BGMswitch)
			{
				PlayerPrefs.SetInt("BGMSWITCH",0);
				_toggleBGM.SetActive(false);
				_BGMswitch = false;
			}
			else
			{
				PlayerPrefs.SetInt("BGMSWITCH",1);
				_toggleBGM.SetActive(true);
				_BGMswitch = true;
			}
			SoundControls.Instance.SetUpSounds();
		}
		public void SwitchSFX()
		{
			SoundControls.Instance._buttonClick.Play();
			if(_SFXswitch)
			{
				PlayerPrefs.SetInt("SFXSWITCH",0);
				_toggleSFX.SetActive(false);
				_SFXswitch = false;
			}
			else
			{
				PlayerPrefs.SetInt("SFXSWITCH",1);
				_toggleSFX.SetActive(true);
				_SFXswitch = true;
			}
			SoundControls.Instance.SetUpSounds();
		}
        public void BackButtonClick()
        {
            if (_DisableBackButton)
            {
                Debug.LogError(D.ERROR + " SPAM IS BLOCKED BY SETTINGS");
                return;
            }
            _DisableBackButton = true;
            InteractiveCanvas.interactable = false;

            SoundControls.Instance._buttonClick.Play();
            Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeScene { Scene = EScene.TitleRoot });
        }
	}

}