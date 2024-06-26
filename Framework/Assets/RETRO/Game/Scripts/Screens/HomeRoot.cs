﻿using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UniRx;

using Common;
using Common.Signal;
using Framework;

namespace Synergy88 {
	
	public class HomeRoot : Scene {

		private Dictionary<EButtonType, Action> buttonMap;

		public GameObject _tutorialScreen;
		public void _EnableTutorial(bool _switch)
		{
			_tutorialScreen.SetActive(_switch);
			SoundControls.Instance._buttonClick.Play();
		}

        void Awake() {
			base.Awake();
			this.buttonMap = new Dictionary<EButtonType, Action>();
		}

		 void Start()
        {
            /*
			Scene.LoadAdditive<GameRoot>(EScene.Game);
			base.Start();
			// add button handlers
			this.AddButtonHandler(EButtonType.MoreGames, (ISignalParameters parameters) => {
				this.LoadScene<MoreGamesRoot>(EScene.MoreGames);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Currency, (ISignalParameters parameters) => {

				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Help, (ISignalParameters parameters) => {
				// dummy
				//this.LoadScene<ResultsRoot>(EScene.Results);
				//this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				_EnableTutorial(true);
			});

			this.AddButtonHandler(EButtonType.Leaderboard, (ISignalParameters parameters) => {
			});

			this.AddButtonHandler(EButtonType.Shop, (ISignalParameters parameters) => {
				this.LoadScene<ShopRoot>(EScene.Shop);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);

				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Play, (ISignalParameters parameters) => {
				this.LoadScene<GameRoot>(EScene.Game);
				//S88Signals.ON_GAME_START.Dispatch();
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);

				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.Settings, (ISignalParameters parameters) => {
				this.LoadScene<SettingsRoot>(EScene.Settings);
				this.LoadSceneAdditive<BackRoot>(EScene.Game);
				this.LoadSceneAdditive<BackRoot>(EScene.Back);

				SoundControls.Instance._buttonClick.Play();
			});*/
		}
        
	}

}