using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common;
using Common.Query;
using Common.Signal;

namespace Synergy88 {

	public class BackRoot : S88Scene {

		private Dictionary<EButtonType, Action> buttonMap;

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

			this.AddButtonHandler(EButtonType.Back, (ISignalParameters parameters) => {
				//EScene prev = QuerySystem.Query<EScene>(QueryIds.PreviousScene);

				this.LoadScene<HomeRoot>(EScene.Home);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Currency);
				this.LoadSceneAdditive<CurrencyRoot>(EScene.Game);
				SoundControls.Instance._buttonClick.Play();
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