using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Query;
using Common.Signal;

public enum EScene {
	Invalid,

	System,

	Splash,
	Preloader,
	Login,
	Home,

	Coins,
	Shop,
	Settings,

	// gameplay related
	Game,
	Results,

	// overlay
	Back,
	Currency,

	Background,

	MoreGames,

	Max,
};

namespace Synergy88 {

	public class S88Scene : MonoBehaviour {
		
		[SerializeField]
		protected EScene sceneType;
		protected Dictionary<EButtonType, Action<ISignalParameters>> buttonMap;

		private static Dictionary<EScene, GameObject> cachedScenes = new Dictionary<EScene, GameObject>(); 

		protected virtual void Awake() {
			if (GameObject.FindObjectOfType<SystemRoot>() == null) {
				SceneManager.LoadScene(EScene.System.ToString(), LoadSceneMode.Additive);
			}

			if (GameObject.FindObjectOfType<BackgroundRoot>() == null) {
				SceneManager.LoadScene(EScene.Background.ToString(), LoadSceneMode.Additive);
			}

			this.buttonMap = new Dictionary<EButtonType, Action<ISignalParameters>>();
			cachedScenes[this.sceneType] = this.gameObject;
		}

		protected virtual void Start() {
		}

		protected virtual void OnEnable() {
			S88Signals.ON_CLICKED_BUTTON.AddListener(this.OnClickedButton);
		}

		protected virtual void OnDisable() {
			S88Signals.ON_CLICKED_BUTTON.RemoveListener(this.OnClickedButton);
		}

		protected virtual void OnDestroy() {
			this.buttonMap.Clear();
			this.buttonMap = null;
			cachedScenes[this.sceneType] = null;
			cachedScenes.Remove(this.sceneType);
		}

		protected void LoadScene<T>(EScene scene) where T : S88Scene {
			S88Scene.Load<T>(scene);
		}

		protected void LoadSceneAdditive<T>(EScene scene) where T : S88Scene {
			S88Scene.LoadAdditive<T>(scene);
		}
		
		protected void AddButtonHandler(EButtonType button, Action<ISignalParameters> action) {
			this.buttonMap[button] = action;
		}

		#region Signals

		private void OnClickedButton(ISignalParameters parameters) {
			EButtonType button = (EButtonType)parameters.GetParameter(S88Params.BUTTON_TYPE);

			if (this.buttonMap.ContainsKey(button) && this.gameObject.activeSelf) {
				Debug.LogFormat("S88Scene::OnClickedButton Button:{0}\n", button);
				this.buttonMap[button](parameters);
			}
		}

		#endregion

		public static void Load<T>(EScene scene) where T : S88Scene {
			Signal signal = S88Signals.ON_LOAD_SCENE;
			signal.ClearParameters();
			signal.AddParameter(S88Params.SCENE_NAME, scene);
			signal.Dispatch();
			HideScenes(scene);

			Action<EScene, string> CheckUnload = (EScene target, string key) => {
				if (target != QuerySystem.Query<EScene>(key)) {
					UnloadScene(target);
				}
			};

			if (scene != EScene.Splash && scene != EScene.Login) {
				CheckUnload(EScene.Splash, QueryIds.CurrentScene);
				CheckUnload(EScene.Login, QueryIds.CurrentScene);
			}
			LoadAdditive<T>(scene);
			if(scene == EScene.Game)
			{
				S88Signals.ON_GAME_START.Dispatch();
			}
		}

		public static void LoadAdditive<T>(EScene scene) where T : S88Scene {
			if (ShowScene<T>(scene) == false) {
				SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
			}
		}

		private static bool ShowScene<T>(EScene scene) where T : S88Scene {
			if (cachedScenes.ContainsKey(scene)) {
				cachedScenes[scene].gameObject.SetActive(true);
				return true;
			}

			return false;
		}

		private static void HideScenes(EScene except) {
			for (EScene scene = EScene.Invalid; scene < EScene.Max; scene++) {
				if (scene == EScene.System) continue;
				if (scene == EScene.Background) continue;
				if (scene == except) continue;
				if (!cachedScenes.ContainsKey(scene)) continue;

				cachedScenes[scene].gameObject.SetActive(false);
			}
		}

		public static void UnloadScene(EScene scene) {
			if (cachedScenes.ContainsKey(scene)) {
				GameObject.Destroy(cachedScenes[scene].gameObject);
				cachedScenes.Remove(scene);
			}
		}
	}

}

