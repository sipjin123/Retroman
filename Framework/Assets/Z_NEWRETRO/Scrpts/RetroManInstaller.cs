﻿
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using uPromise;

using UniRx;

using Sirenix.OdinInspector;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Utils;

using Framework;

using Sandbox.Audio;
using Sandbox.Facebook;
using Sandbox.FGCAutomation;
using Sandbox.Popup;
using Sandbox.Preloader;
using Sandbox.RGC;
using Sandbox.Services;
using Synergy88;

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
        private const string ON_SETTINGS = "ON_SETTINGS";
        private const string ON_GAME = "ON_GAME";
        private const string ON_RESULTS = "ON_RESULTS";
        private const string ON_SHOP = "ON_SHOP";

        // Fsm
        [SerializeField]
        private Fsm Fsm;

        private MessageBroker _RetroMessageBroker = new MessageBroker();

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
        private static bool HasAttemptedFGCLogin;


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
        bool skipProcess;

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


            this._RetroMessageBroker.Receive<LaunchGamePlay>().Subscribe(_ =>
            {
                skipProcess = true;
                Fsm.SendEvent(ON_GAME);
            }).AddTo(this);



            this._RetroMessageBroker.Receive<PressBackButtonINIT>().Subscribe(_ =>
            {
                Debug.LogError("recive signal back");
                Debug.LogError(Fsm.GetCurrentStateName());
                switch (Fsm.GetCurrentStateName())
                {
                    case "game":
                        Debug.LogError("BackButton Game");
                        this._RetroMessageBroker.Publish(new PressBackButton { BackButtonType = BackButtonType.SceneIsGame });
                        break;
                    case "shop":

                        this._RetroMessageBroker.Publish(new PressBackButton { BackButtonType = BackButtonType.SceneIsShop });
                        Debug.LogError("BackButton Shop");
                        break;
                    case "title":

                        this._RetroMessageBroker.Publish(new PressBackButton { BackButtonType = BackButtonType.SceneIsTitle });
                        Debug.LogError("BackButton Title");
                        break;
                    case "settings":

                        this._RetroMessageBroker.Publish(new PressBackButton { BackButtonType = BackButtonType.SceneIsSettings });
                        Debug.LogError("BackButton Settings");
                        break;
                }
            }).AddTo(this);

            this._RetroMessageBroker.Receive<ChangeScene>().Subscribe(_ =>
            {
                Debug.LogError(D.LOG + " :: My current fsm is :: " + Fsm.GetCurrentStateName());
                switch (_.Scene)
                {
                    case EScene.GameRoot:
                        this._RetroMessageBroker.Publish(new ToggleCoins { IfActive = false });
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
                    case EScene.SettingsRoot:
                        this._RetroMessageBroker.Publish(new ToggleCoins { IfActive = false });
                        Fsm.SendEvent(ON_SETTINGS);
                        break;
                }
            }).AddTo(this);
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
            FsmState settings = Fsm.AddState("settings");


            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                string splashMovieScene = "SplashMovie";

                Preloaders preloaders = Preloaders.Preloader001;

                Promise.All(Scene.LoadScenePromise<SplashMovieRoot>(splashMovieScene))
                    .Then(_ => 
                    {
                        FScene.GetScene<Framework.SystemRoot>(EScene.System).ToggleBlackPanel(true);
                    })
                    .Then(_ => FSceneObject.GetScene<SplashMovieRoot>(splashMovieScene).Wait(3))
                    .Then(_ =>
                    {
                        FScene.GetScene<Framework.SystemRoot>(EScene.System).ToggleLoadPanel(true);
                    })
                    .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader = Scene.GetSceneRoot<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader.LoadLoadingScreenPromise(preloaders))
                    .Then(_ => Fsm.SendEvent(ON_PRELOAD));
            }));
            
            preload.AddAction(new FsmDelegateAction(preload, delegate (FsmState owner)
            {
                string audio = "Audio";
                Promise.All(Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => Scene.LoadScenePromise<AudioRoot>(audio))
                    .Then(_ => Scene.LoadScenePromise<ServicesRoot>(EScene.Services))
                    .Then(_ => Scene.LoadScenePromise<PopupCollectionRoot>(EScene.PopupCollection))
                    .Then(_ => Scene.LoadSceneAdditivePromise<BackgroundRoot>(EScene.Background))
                    .Then(_ => Factory.Get<DataManagerService>().InjectBroker(this._RetroMessageBroker))
                    .Then(_ => SoundControls.Instance.SetupMessageBroker(this._RetroMessageBroker))
                    .Then(_ => Scene.LoadScenePromise<CoinsRoot>(EScene.CoinsRoot))
                    .Then(_ => Fsm.SendEvent(ON_TITLE))
                      .Then(_ =>
                      {
                          if (!HasAttemptedFGCLogin)
                          {
                              OnConnectToFGCApp signal;
                              this.Publish(signal);
                              HasAttemptedFGCLogin = true;
                              this.Receive<OnFacebookLoginFailedSignal>()
                                  .Subscribe(x =>
                                  {
                                      Debug.LogError($"{D.ERROR} Facebook Login failed");
                                      CloseAllPopups();
                                  }).AddTo(this);
                          }
                          Debug.LogError(D.LOBBY + "Title Scene Init Loading");
                      })
                    .Then(_ => Preloader.UnloadScenePromise(EScene.Background))
                    .Then(_ => Preloader.UnloadScenePromise(EScene.Preloader));
            }));

            title.AddAction(new FsmDelegateAction(title, delegate (FsmState owner)
            {
                
                Promise.AllSequentially(Scene.EndFramePromise)
                    .Then(_ => Scene.LoadSceneAdditivePromise<BackgroundRoot>(EScene.Background))
                    .Then(_ =>
                    {
                        FScene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
                    })
#if ENABLE_FGC_TRACKING
                    .Then(_ => FScene.GetScene<FGCAutomationRoot>(EScene.FGCAutomation) == null ? 
                        Scene.LoadSceneAdditivePromise<FGCAutomationRoot>(EScene.FGCAutomation) : 
                        Scene.EndFramePromise())
#endif
                    .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Scene.LoadSceneAdditivePromise<GameRoot>(EScene.GameRoot))
                    .Then(_ => Scene.LoadSceneAdditivePromise<TitleRoot>(EScene.TitleRoot))
                    .Then(_=>
                    {
                        FScene.GetScene<Framework.SystemRoot>(EScene.System).ToggleBlackPanel(false);
                        FScene.GetScene<Framework.SystemRoot>(EScene.System).ToggleLoadPanel(false);
                        Factory.Get<DataManagerService>().IfCanBack = true;
                        this._RetroMessageBroker.Publish(new ShowVersion { IfActive = true });
                        this._RetroMessageBroker.Publish(new ToggleCoins { IfActive = true });
                        Debug.LogError(D.LOBBY+"Title Scene Finished Loading");
                    })
                    .Then(_ => Scene.UnloadScenePromise(EScene.Preloader))
                    .Then(_ => Scene.UnloadScenePromise(EScene.Background));
            }));

            game.AddAction(new FsmDelegateAction(game, delegate (FsmState owner)
            {
                if (skipProcess == false)
                {
                    Promise.AllSequentially(Scene.EndFramePromise)
                        .Then(_ => Scene.LoadSceneAdditivePromise<BackgroundRoot>(EScene.Background))
                        .Then(_ =>
                        {
                            FScene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
                        })
                        .Then(_=> Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                        .Then(_ =>
                        {
                            Time.timeScale = 1;
                        })
                        .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).LoadLoadingScreenPromise(Preloaders.Preloader001))
                        .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).FadeInLoadingScreenPromise())
                        .Then(_ => Scene.LoadSceneAdditivePromise<GameRoot>(EScene.GameRoot))
                        .Then(_ =>
                        {
                            this._RetroMessageBroker.Publish(new LaunchGamePlay());
                        })
                        .Then(_ => Scene.UnloadScenePromise(EScene.Background))
                        .Then(_ => Scene.UnloadScenePromise(EScene.Preloader));
                }
                else
                {
                    skipProcess = false;
                }
            }));

            shop.AddAction(new FsmDelegateAction(shop, delegate (FsmState owner)
            {
                Promise.AllSequentially(Scene.EndFramePromise)
                        .Then(_ => Scene.LoadSceneAdditivePromise<BackgroundRoot>(EScene.Background))
                        .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                        .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).LoadLoadingScreenPromise(Preloaders.Preloader001))
                        .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).FadeInLoadingScreenPromise())
                        .Then(_ => Scene.LoadScenePromise<ShopRoot>(EScene.ShopRoot)).
                        Then(_=> 
                        {
                            _RetroMessageBroker.Publish(new ToggleCoins { IfActive = true });
                        })
                        .Then(_ => Scene.UnloadScenePromise(EScene.Background))
                        .Then(_ => Scene.UnloadScenePromise(EScene.Preloader));

            }));
            settings.AddAction(new FsmDelegateAction(settings, delegate (FsmState owner)
            {
                Promise.AllSequentially(Scene.EndFramePromise)
                    .Then(_ => Scene.LoadSceneAdditivePromise<BackgroundRoot>(EScene.Background))
                    .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).LoadLoadingScreenPromise(Preloaders.Preloader001))
                    .Then(_ => FScene.GetScene<PreloaderRoot>(EScene.Preloader).FadeInLoadingScreenPromise())
                    .Then(_ => Scene.LoadScenePromise<SettingsRoot>(EScene.SettingsRoot))
                    .Then(_ => Scene.UnloadScenePromise(EScene.Background))
                    .Then(_ => Scene.UnloadScenePromise(EScene.Preloader));
            }));


            idle.AddTransition(ON_SPLASH, splash);
            idle.AddTransition(ON_PRELOAD, preload);
            idle.AddTransition(ON_TITLE, title);

            splash.AddTransition(ON_PRELOAD, preload);
            preload.AddTransition(ON_TITLE, title);

            title.AddTransition(ON_GAME, game);
            title.AddTransition(ON_SHOP, shop);
            title.AddTransition(ON_TITLE, title);
            title.AddTransition(ON_SETTINGS, settings);

            settings.AddTransition(ON_TITLE, title);


            game.AddTransition(ON_SHOP, shop);
            game.AddTransition(ON_TITLE, title);
            game.AddTransition(ON_GAME, game);

            shop.AddTransition(ON_GAME, game);
            shop.AddTransition(ON_TITLE, title);

            results.AddTransition(ON_TITLE, title);
            results.AddTransition(ON_GAME, game);

            Fsm.Start(IDLE);
        }

        private async void CloseAllPopups()
        {
            await DelayAction(1.5f, () =>
            {
                OnCloseActivePopup closeSignal;
                closeSignal.All = true;
                this.Publish(closeSignal);
            });
        }

        private async Task DelayAction(float delay, Action action)
        {
            await new TimeSpan(0, 0, 0, (int)delay);
            action?.Invoke();
        }

        #endregion
    }
}