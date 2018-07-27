using UnityEngine;
using System.Collections;

using Common.Signal;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Framework;
using Retroman;
using Common.Utils;

namespace Synergy88 {
	
	public class ResultsRoot : Scene {

		public GameObject _WAtchVidButton;

		public Text HiScore1,HiScore2, CScore1,CScore2;

		protected override void Awake() {
			S88Signals.ON_CLICKED_BUTTON.AddListener(ResetGame);
			base.Awake();
		}

		protected override void Start() {
			base.Start();
            /*
			this.AddButtonHandler(EButtonType.Score, (ISignalParameters parameters) => {
				// score button clicked
			});

			this.AddButtonHandler(EButtonType.Home, (ISignalParameters parameters) => {
				// home button clicked
				// load preloader here
				this.LoadScene<HomeRoot>(EScene.Home);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Ads, (ISignalParameters parameters) => {

				S88Signals.ON_SHOW_UNITY_ADS.Dispatch();
				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Leaderboard, (ISignalParameters parameters) => {
				// leaderboards button clicked
			});*/


            Debug.LogError("CURRENCY");
            Debug.LogError("BACK");
            Debug.LogError("GAME");
            Debug.LogError("SHOP");
            /*
            this.AddButtonHandler(EButtonType.Shop, (ISignalParameters parameters) => {



                // shop button clicked
                this.LoadScene<ShopRoot>(EScene.Shop);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				SoundControls.Instance._buttonClick.Play();
			});*/


		}

		protected override void OnEnable() {
			try{
                Factory.Get<DataManagerService>().GameControls._resultCharParent.SetActive(true);
			}catch{}
			S88Signals.ON_CLICKED_BUTTON.AddListener(ResetGame);

				

			CScore1.text = ""+PlayerPrefs.GetInt("curSkor",0);
			CScore2.text = ""+PlayerPrefs.GetInt("curSkor",0);
			HiScore1.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);
			HiScore2.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);

			base.OnEnable();

		}

		protected override void OnDisable() {
			try{
                Factory.Get<DataManagerService>().GameControls._resultCharParent.SetActive(false);
			}catch{}
			S88Signals.ON_CLICKED_BUTTON.RemoveListener(ResetGame);
			base.OnDisable();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}
		void ResetGame(ISignalParameters param)
		{
			if(param.GetParameter(S88Params.BUTTON_TYPE).ToString() == "Refresh")
            {
                Debug.LogError("GAME");
                Debug.LogError("CURRENCY");
               // Scene.Load<GameRoot>(EScene.Game);
			////	Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);

			}
		}

	}

}