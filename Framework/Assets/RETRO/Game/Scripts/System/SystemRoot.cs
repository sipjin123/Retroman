﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Framework;
using Scene = Framework.Scene;

namespace Synergy88 {
        public class SystemRoot : MonoBehaviour {

		public GameObject _cameraObject;


		[SerializeField]
		private EScene currentScene	= EScene.Invalid;

		[SerializeField]
		private EScene previousScene = EScene.Invalid;
		
		[RuntimeInitializeOnLoadMethod]
		private static void OnRuntimeMethodLoad() {
			// start the scene with splash
			//if (!Debug.isDebugBuild) {
			//	Scene.Load<SplashRoot>(EScene.Splash);
			//}
		}

		private void Awake() {
			S88Signals.ON_GAME_OVER.AddListener(OnGameOver);
			S88Signals.ON_GAME_START.AddListener(OnGameStartup);
			S88Signals.ON_LOAD_SCENE.AddListener(this.OnLoadScene);
			S88Signals.ON_LOAD_SPLASH.AddListener(this.OnLoadSplash);
			S88Signals.ON_SPLASH_DONE.AddListener(this.OnSplashDone);	
			S88Signals.ON_LOGIN_DONE.AddListener(this.OnLoginDone);	

			QuerySystem.RegisterResolver(QueryIds.CurrentScene, delegate(IQueryRequest request, IMutableQueryResult result) {
				result.Set(this.currentScene);	
			});

			QuerySystem.RegisterResolver(QueryIds.PreviousScene, delegate(IQueryRequest request, IMutableQueryResult result) {
				result.Set(this.previousScene);	
			});

			// prepare Fsm
			this.PrepareSceneFsm();
		}

		void Start()
		{
			S88Signals.ON_LOAD_SPLASH.Dispatch();
		}
		
		private void OnDestroy() {
			S88Signals.ON_GAME_OVER.RemoveListener(OnGameOver);
			S88Signals.ON_GAME_START.RemoveListener(OnGameStartup);
			S88Signals.ON_LOAD_SCENE.RemoveListener(this.OnLoadScene);
			S88Signals.ON_LOAD_SPLASH.RemoveListener(this.OnLoadSplash);
			S88Signals.ON_SPLASH_DONE.RemoveListener(this.OnSplashDone);
			S88Signals.ON_LOGIN_DONE.RemoveListener(this.OnLoginDone);	

			QuerySystem.RemoveResolver(QueryIds.CurrentScene);
			QuerySystem.RemoveResolver(QueryIds.PreviousScene);
		}

		#region Scene Fsm

		// initial state
		private const string IDLE = "Idle";

		// events
		private const string START_SPLASH_SCREEN = "StartSplashScreen";
		private const string START_PRELOAD = "StartPreload";
		private const string START_LOGIN = "StartLogin";
		private const string FINISHED = "Finished";

		private Fsm fsm;

		private void PrepareSceneFsm() {
			this.fsm = new Fsm("SceneFsm");

			// states
			FsmState idle = fsm.AddState(IDLE);
			FsmState splash = fsm.AddState("splash");
			FsmState preload = fsm.AddState("preload");
			FsmState login = fsm.AddState("login");
			FsmState done = fsm.AddState("done");

			// actions
			idle.AddAction(new FsmDelegateAction(idle, delegate(FsmState owner) {
				Debug.Log("SceneFsm::Idle State\n");
			}));

			splash.AddAction(new FsmDelegateAction(splash, delegate(FsmState owner) {
				Debug.Log("SceneFsm::Splash State\n");

                Debug.LogError("SPLASH"); //Scene.Load<SplashRoot>(EScene.Splash);
			}));

			preload.AddAction(new FsmDelegateAction(preload, delegate(FsmState owner) {
				Debug.Log("SceneFsm::Preload State\n");
				// load preloader here
				owner.SendEvent(START_LOGIN);
			}));

			login.AddAction(new FsmDelegateAction(login, delegate(FsmState owner) {
				Debug.Log("SceneFsm::Login State\n");
				//Scene.Load<LoginRoot>(EScene.Login);
                Debug.LogError("HOME");
            }));

			done.AddAction(new FsmDelegateAction(done, delegate(FsmState owner) {
				Debug.Log("SceneFsm::Done State\n");
				//Scene.Load<HomeRoot>(EScene.Home);
				//Scene.LoadAdditive<CurrencyRoot>(EScene.Currency);
                Debug.LogError("HOME");
                Debug.LogError("CURREENCY");
            }));

			// transitions
			idle.AddTransition(START_SPLASH_SCREEN, splash);
			splash.AddTransition(START_PRELOAD, preload);
			preload.AddTransition(START_LOGIN, login);
			login.AddTransition(FINISHED, done);

			// auto start fsm
			this.fsm.Start(IDLE);
		}

		#endregion

		#region Signals

		private void OnLoadScene(ISignalParameters parameters) {
			EScene sceneName = (EScene)parameters.GetParameter(S88Params.SCENE_NAME);
			if (this.currentScene == EScene.Invalid) {
				this.currentScene = sceneName;
			}
			else {
				this.previousScene = this.currentScene;
				this.currentScene = sceneName;
			}
		}

		private void OnLoadSplash(ISignalParameters parameters) {
			this.fsm.SendEvent(START_SPLASH_SCREEN);
		}

		private void OnSplashDone(ISignalParameters parameters) {

			this.fsm.SendEvent(START_PRELOAD);
			S88Signals.ON_FACEBOOK_LOGIN_SUCCESSFUL.Dispatch();//B0NTA
		}

		private void OnLoginDone(ISignalParameters parameters) {
			this.fsm.SendEvent(FINISHED);
			_cameraObject.SetActive(false);
		}

		#endregion

		void OnGameStartup(ISignalParameters parameters)
		{
		}
		void OnGameOver(ISignalParameters parameters)
		{
            Debug.LogError("CALL GAME SCENE");
			//Scene.LoadAdditive<GameRoot>(EScene.Game);
		}
	}

}