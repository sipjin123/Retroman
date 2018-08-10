using UnityEngine;
using System.Collections;

using Common.Signal;
using Framework;
using Common.Utils;

namespace Retroman {
	
	public class SettingsRoot : Scene
    {

		public GameObject _toggleBGM, _toggleSFX;
		bool _BGMswitch, _SFXswitch;
		public GameObject _CreditsWindow;

        void Awake() {
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
		}

        void Start() {

            /*

			this.AddButtonHandler(EButtonType.Credits, (ISignalParameters parameters) => {


				_CreditsWindow.SetActive(true);
				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Restore, (ISignalParameters parameters) => {
				// toggle interstitals
				S88Signals.ON_TOGGLE_INTERSTITIAL_ADS.Dispatch();

				// test unity ads
				S88Signals.ON_SHOW_UNITY_ADS.ClearParameters();
				S88Signals.ON_SHOW_UNITY_ADS.AddParameter(S88Params.UNITY_ADS_REGION, "001Region");
				S88Signals.ON_SHOW_UNITY_ADS.Dispatch();
				SoundControls.Instance._buttonClick.Play();
			});*/
		}

        public void CreditsOnClick()
        {
            _CreditsWindow.SetActive(true);
            SoundControls.Instance._buttonClick.Play();
        }

        void OnEnable() {
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
            SoundControls.Instance._buttonClick.Play();

            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleSetting { IfActive = false });
        }
	}

}