using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uPromise;

using UniRx;

using Sirenix.OdinInspector;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Utils;

using Framework;

using Sandbox.Audio;
using Sandbox.Popup;
using Sandbox.Preloader;
using Sandbox.Services;

namespace Sandbox.SceneFlow
{
    using FScene = Framework.Scene;
    using FSceneObject = Framework.SceneObject;

    public class SceneFlowInstaller : ConcreteInstaller, IInstaller
    {
        public static readonly string CURR_SCENE = "CurrentScene";
        public static readonly string PREV_SCENE = "PreviousScene";
        public static readonly string FSM_STATE = "SystemState";
        public static readonly string PRELOADER = "Preloader";
        public static readonly string POPUP = "PopupCollection";
        
        // initial state
        private const string IDLE = "Idle";

        // events
        private const string ON_SPLASH = "ON_SPLASH";
        private const string ON_PRELOAD = "ON_PRELOAD";

        // Fsm
        private Fsm Fsm;

        [SerializeField, ShowInInspector]
        private EScene _CurrentScene = EScene.Invalid;
        public EScene CurrentScene
        {
            get { return _CurrentScene; }
            private set { _CurrentScene = value; }
        }

        [SerializeField, ShowInInspector]
        private EScene _PreviousScene = EScene.Invalid;
        public EScene PreviousScene
        {
            get { return _PreviousScene; }
            private set { _PreviousScene = value; }
        }

        private PreloaderRoot Preloader;
        private PopupCollectionRoot Popup;
        
        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(CURR_SCENE);
            QuerySystem.RemoveResolver(PREV_SCENE);
            QuerySystem.RemoveResolver(FSM_STATE);
            QuerySystem.RemoveResolver(PRELOADER);
            QuerySystem.RemoveResolver(POPUP);
        }

        public override void Install()
        {
            base.Install();

            SetupQueries();
            SetupListeners();
            PrepareSceneFsm();
        }

        #region Queries

        private void SetupQueries()
        {
            QuerySystem.RegisterResolver(CURR_SCENE, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(CurrentScene);
            });

            QuerySystem.RegisterResolver(PREV_SCENE, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(PreviousScene);
            });

            QuerySystem.RegisterResolver(FSM_STATE, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Fsm.GetCurrentStateName());
            });

            QuerySystem.RegisterResolver(PRELOADER, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Preloader);
            });

            QuerySystem.RegisterResolver(POPUP, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Popup);
            });
        }

        #endregion

        #region Listeners

        private void SetupListeners()
        {
            // Setup Listeners
            this.Receive<OnLoadSceneSignal>()
                .Where(_ => _.IsRootScene)
                .Subscribe(_ => OnLoadScene(_.Scene, _.SceneName))
                .AddTo(this);

            this.Receive<OnUnloadSceneSignal>()
               .Subscribe(_ => OnUnloadScene(_.SceneName))
               .AddTo(this);
        }

        #endregion

        #region Signals

        private void OnLoadScene(EScene scene, string sceneName)
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene;
        }

        private void OnUnloadScene(string sceneName)
        {     
        }

        #endregion

        #region Scene Flow Fsm
        private void PrepareSceneFsm()
        {
            Fsm = new Fsm("SceneFsm");

            // states
            FsmState idle = Fsm.AddState(IDLE);
            FsmState splash = Fsm.AddState("splash");
            FsmState preload = Fsm.AddState("preload");

            // actions
            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                string splashScene = "Splash";
                string splashMoveScene = "SplashMovie";

                /*
                Preloaders preloaders =
                    Preloaders.Preloader001 |
                    Preloaders.Preloader002 |
                    Preloaders.Preloader003 |
                    Preloaders.Preloader004;
                    //*/

                Preloaders preloaders = Preloaders.Preloader001;

                // idle state
                Promise.All(Scene.LoadScenePromise<SplashRoot>(splashScene))
                    .Then(_ => FSceneObject.GetScene<SplashRoot>(splashScene).Wait())
                    .Then(_ => Scene.LoadScenePromise<SplashMovieRoot>(splashMoveScene))
                    .Then(_ => FSceneObject.GetScene<SplashMovieRoot>(splashMoveScene).Wait())
                    .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader = Scene.GetSceneRoot<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader.LoadLoadingScreenPromise(preloaders))
                    .Then(_ => Fsm.SendEvent(ON_PRELOAD));
            }));

            splash.AddAction(new FsmDelegateAction(splash, delegate (FsmState owner)
            {
                // state 1
            }));

            preload.AddAction(new FsmDelegateAction(preload, delegate (FsmState owner)
            {
                // state 2
                Promise.All(Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => Scene.LoadScenePromise<AudioRoot>(EScene.Audio))
                    .Then(_ => Scene.LoadScenePromise<ServicesRoot>(EScene.Services))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    //.Then(_ => Scene.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => Preloader.LoadLoadingScreenPromise(Preloaders.Preloader002))
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ =>
                    {
                        // Sample async operation on loading scenes
                        /*
                        AsyncOperation operation = SceneManager.LoadSceneAsync("Shooting", LoadSceneMode.Additive);
                        operation
                            .AsAsyncOperationObservable(progress: Preloader.GetNotifier())
                            .Take(1)
                            .Subscribe();
                        //*/

                        // Sample async operation WWW 
                        //*
                        ObservableWWW
                            //.Get("http://google.com/", progress: Preloader.GetNotifier())
                            .Get("http://synergy88digital.com/", progress: Preloader.GetNotifier())
                            .Take(1)
                            .Subscribe();
                        //*/

                        // Sample sequence operation
                        /*
                        var query = from google in ObservableWWW.Get("http://google.com/")
                                    from bing in ObservableWWW.Get("http://bing.com/")
                                    from unknown in ObservableWWW.Get(google + bing)
                                    select new { google, bing, unknown };

                        var cancel = query.Do(Preloader.GetNotifier());
                        //*/

                        // Sample parallel operation
                        /*
                        Observable.WhenAll(
                            ObservableWWW.Get("http://google.com/"),
                            ObservableWWW.Get("http://google.com/"),
                            ObservableWWW.Get("http://google.com/"),
                            ObservableWWW.Get("http://google.com/"),
                            ObservableWWW.Get("http://google.com/"),
                            ObservableWWW.Get("http://google.com/"));
                        //*/
                    })
                    .Then(_ => Preloader.WaitNotifier())
                    .Then(_ => Fsm.SendEvent(ON_PRELOAD))
                    .Then(_ => Debug.LogErrorFormat("Preload Done.\n"));
            }));

            // transitions
            idle.AddTransition(ON_SPLASH, splash);
            idle.AddTransition(ON_PRELOAD, preload);

            splash.AddTransition(ON_PRELOAD, preload);

            // auto start fsm
            Fsm.Start(IDLE);
        }

        #endregion
    }
}