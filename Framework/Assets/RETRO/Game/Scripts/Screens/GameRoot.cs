using UnityEngine;

using System.Collections;
using Common.Signal;
using Framework;
using Common.Utils;

namespace Synergy88 {
	
	public class GameRoot : Scene {

		public GameObject _ToSpawnGameStartupScene;
		public GameObject _CurrentGameStartupScene;
		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();
		}

		protected override void OnEnable() {
			S88Signals.ON_GAME_START.AddListener(OnGameSTartup);
			base.OnEnable();
		}

		protected override void OnDisable() {
			S88Signals.ON_GAME_START.RemoveListener(OnGameSTartup);
			base.OnDisable();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}


		private void OnGameSTartup(ISignalParameters parameters) {
			Destroy(_CurrentGameStartupScene);
			GameObject temp = Instantiate(_ToSpawnGameStartupScene,transform.position,Quaternion.identity) as GameObject;
			_CurrentGameStartupScene = temp;
			temp.transform.parent = transform;
		}
	}
		
}