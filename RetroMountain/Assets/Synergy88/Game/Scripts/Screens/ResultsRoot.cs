using UnityEngine;
using System.Collections;

using Common.Signal;
using UnityEngine.UI;

namespace Synergy88 {
	
	public class ResultsRoot : S88Scene {

		public GameObject _Avatar;

		public Text HiScore1,HiScore2, CScore1,CScore2;

		protected override void Awake() {
			S88Signals.ON_CLICKED_BUTTON.AddListener(ResetGame);
			base.Awake();
		}

		protected override void Start() {
			base.Start();

			this.AddButtonHandler(EButtonType.Score, (ISignalParameters parameters) => {
				// score button clicked
			});

			this.AddButtonHandler(EButtonType.Home, (ISignalParameters parameters) => {
				// home button clicked
				// load preloader here
				this.LoadScene<HomeRoot>(EScene.Home);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				GameControls.Instance._resultCharParent.SetActive(false);
			});

			this.AddButtonHandler(EButtonType.Ads, (ISignalParameters parameters) => {

				S88Signals.ON_SHOW_UNITY_ADS.Dispatch();
			});

			this.AddButtonHandler(EButtonType.Leaderboard, (ISignalParameters parameters) => {
				// leaderboards button clicked
			});

			this.AddButtonHandler(EButtonType.Shop, (ISignalParameters parameters) => {
				// shop button clicked
				this.LoadScene<ShopRoot>(EScene.Shop);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
			});

			this.AddButtonHandler(EButtonType.Refresh, (ISignalParameters parameters) => {
				// refresh button clicked
			});

			this.AddButtonHandler(EButtonType.Upload, (ISignalParameters parameters) => {
				// update/share button clicked
			});
		}

		protected override void OnEnable() {
			S88Signals.ON_CLICKED_BUTTON.AddListener(ResetGame);

			foreach(Transform child in _Avatar.transform)
			{
				child.gameObject.SetActive(false);
			}
			_Avatar.transform.GetChild( PlayerPrefs.GetInt("CurrentCharacter",0) ).gameObject.SetActive(true);
		
			CScore1.text = ""+PlayerPrefs.GetInt("curSkor",0);
			CScore2.text = ""+PlayerPrefs.GetInt("curSkor",0);
			HiScore1.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);
			HiScore2.text = "Best "+PlayerPrefs.GetInt("hiSkor",0);

			base.OnEnable();

		}

		protected override void OnDisable() {
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
				S88Scene.Load<GameRoot>(EScene.Game);
				S88Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);

			}
		}

	}

}