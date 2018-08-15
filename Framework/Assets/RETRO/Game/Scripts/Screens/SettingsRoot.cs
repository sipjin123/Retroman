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

            Factory.Get<DataManagerService>().MessageBroker.Receive<PressBackButton>().Subscribe(_ =>
            {
                if (_.BackButtonType == BackButtonType.SceneIsTitle)
                {
                    if (QuerySystem.Query<bool>(QueryIds.IF_SETTINGS_ACTIVE) == true)
                    {
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleCoins { IfActive = false });
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleSetting { IfActive = false });
                    }
                }
            });

             base.Awake();
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
            SoundControls.Instance._buttonClick.Play();

            Factory.Get<DataManagerService>().MessageBroker.Publish(new ToggleSetting { IfActive = false });
        }
	}

}