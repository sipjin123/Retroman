using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UniRx;

using Common;
using Common.Signal;

namespace Synergy88 {
	
	public class HomeRoot : S88Scene {

		private Dictionary<EButtonType, Action> buttonMap;

		protected override void Awake() {
			base.Awake();
			this.buttonMap = new Dictionary<EButtonType, Action>();
		}

		protected override void Start() {
			S88Scene.LoadAdditive<GameRoot>(EScene.Game);
			base.Start();
			// add button handlers
			this.AddButtonHandler(EButtonType.MoreGames, (ISignalParameters parameters) => {
				this.LoadScene<MoreGamesRoot>(EScene.MoreGames);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
			});

			this.AddButtonHandler(EButtonType.Currency, (ISignalParameters parameters) => {
			});

			this.AddButtonHandler(EButtonType.Help, (ISignalParameters parameters) => {
				// dummy
				//this.LoadScene<ResultsRoot>(EScene.Results);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
			});

			this.AddButtonHandler(EButtonType.Leaderboard, (ISignalParameters parameters) => {
			});

			this.AddButtonHandler(EButtonType.Shop, (ISignalParameters parameters) => {
				this.LoadScene<ShopRoot>(EScene.Shop);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
			});

			this.AddButtonHandler(EButtonType.Play, (ISignalParameters parameters) => {
				this.LoadScene<GameRoot>(EScene.Game);
				//S88Signals.ON_GAME_START.Dispatch();
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);

			});

			this.AddButtonHandler(EButtonType.Settings, (ISignalParameters parameters) => {
				this.LoadScene<SettingsRoot>(EScene.Settings);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
			});
		}

		protected override void OnEnable() {
			base.OnEnable();
		}

		protected override void OnDisable() {
			base.OnDisable();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

	}

}