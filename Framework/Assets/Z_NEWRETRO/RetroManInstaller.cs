
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

namespace Retroman
{
    using FScene = Framework.Scene;
    using FSceneObject = Framework.SceneObject;

    public class RetroManInstaller : ConcreteInstaller, IInstaller
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

        private const string ON_TITLE = "ON_TITLE";
        private const string ON_GAME = "ON_GAME";
        private const string ON_RESULTS = "ON_RESULTS";
        private const string ON_SHOP = "ON_SHOP";

        // Fsm
        private Fsm Fsm;

        MessageBroker RetroMessageBroker = new MessageBroker();

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

            WaitForSignals();

            SetupQueries();
            SetupListeners();
            PrepareSceneFsm();
        }

        private void WaitForSignals()
        {
            RetroMessageBroker.Receive<ChangeScene>().Subscribe(_ => 
            {
                Debug.LogError("My current fsm is :: " + Fsm.GetCurrentStateName());
                switch(_.Scene)
                {
                    case EScene.GameRoot:
                        Fsm.SendEvent(ON_GAME);
                        break;
                    case EScene.TitleRoot:
                        Fsm.SendEvent(ON_TITLE);
                        break;
                    case EScene.ResultRoot:
                        Fsm.SendEvent(ON_RESULTS);
                        break;
                    case EScene.ShopRoot:
                        Fsm.SendEvent(ON_SHOP);
                        break;
                }
            }).AddTo(this);
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
            FsmState title = Fsm.AddState("title");
            FsmState game = Fsm.AddState("game");
            FsmState results = Fsm.AddState("results");
            FsmState shop = Fsm.AddState("shop");


            // actions
            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                string splashScene = "Splash";
                string splashMoveScene = "SplashMovie";


                Preloaders preloaders = Preloaders.Preloader001;

                // idle state
                Promise.All(Scene.LoadScenePromise<SplashRoot>(splashScene))
                    .Then(_ => FSceneObject.GetScene<SplashRoot>(splashScene).Wait())
                    .Then(_ => Scene.LoadScenePromise<SplashMovieRoot>(splashMoveScene))
                    .Then(_ => FSceneObject.GetScene<SplashMovieRoot>(splashMoveScene).Wait())
                    .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader = Scene.GetSceneRoot<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader.LoadLoadingScreenPromise(preloaders))
                     //.Then(_ => Fsm.SendEvent(ON_TITLE));
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
                    .Then(_=> Factory.Get<DataManagerService>().InjectBroker(RetroMessageBroker))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => Preloader.LoadLoadingScreenPromise(Preloaders.Preloader001))
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => Fsm.SendEvent(ON_TITLE))
                    .Then(_ => Preloader.UnloadScenePromise(EScene.Preloader));
            }));

            title.AddAction(new FsmDelegateAction(title, delegate (FsmState owner)
            {
                string splashMoveScene = "SplashMovie";
                //Scene.LoadScenePromise<TitleRoot>(EScene.TitleRoot);
               

                Promise.AllSequentially(Scene.EndFramePromise)
                    .Then(_ => Scene.LoadScenePromise<SplashMovieRoot>(splashMoveScene))
                    .Then(_ => Scene.LoadScenePromise<TitleRoot>(EScene.TitleRoot));
            }));

            game.AddAction(new FsmDelegateAction(game, delegate (FsmState owner)
            {
                string splashMoveScene = "SplashMovie";

                
                Promise.AllSequentially(Scene.EndFramePromise)
                    .Then(_ => Scene.LoadScenePromise<SplashMovieRoot>(splashMoveScene))
                    .Then(_ => Scene.LoadScenePromise<GameRoot>(EScene.GameRoot))
                    .Then(_ =>
                    {
                        RetroMessageBroker.Publish(new LaunchGamePlay());
                    });
            }));

            shop.AddAction(new FsmDelegateAction(shop, delegate (FsmState owner)
            {
                Scene.LoadScenePromise<ShopRoot>(EScene.ShopRoot);
            }));


            /*
            title.AddAction(new FsmDelegateAction(title, delegate (FsmState owner)
            {
            }));
            */

            idle.AddTransition(ON_SPLASH, splash);
            idle.AddTransition(ON_PRELOAD, preload);
            idle.AddTransition(ON_TITLE, title);

            splash.AddTransition(ON_PRELOAD, preload);
            preload.AddTransition(ON_TITLE, title);

            title.AddTransition(ON_GAME, game);
            title.AddTransition(ON_SHOP, shop);
            title.AddTransition(ON_TITLE, title);

            game.AddTransition(ON_SHOP, shop);
            game.AddTransition(ON_TITLE, title);
            game.AddTransition(ON_GAME, game);

            shop.AddTransition(ON_GAME, game);

            results.AddTransition(ON_TITLE, title);
            results.AddTransition(ON_GAME, game);

            Fsm.Start(IDLE);
        }

        #endregion
    }
}