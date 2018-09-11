using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using uPromise;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Utils;

namespace Sandbox.RGC
{
    using Framework;

    using Sandbox.Audio;
    using Sandbox.Popup;
    using Sandbox.Preloader;
    using Sandbox.Services;

    public class RGCInstaller : ConcreteInstaller, IInstaller
    {
        public const string PRELOADER_RESOLVER = "PreloaderResolver";

        private PreloaderRoot Preloader;

        public override void Install()
        {
            base.Install();

            string splashScene = "Splash";
            string splashMoveScene = "SplashMovie";
            string titleScene = "Title";
            Preloaders preloaders = Preloaders.Preloader001 | Preloaders.Preloader002 | Preloaders.Preloader003;

            Promise.All(Scene.LoadScenePromise<SplashRoot>(splashScene))
                .Then(_ => SceneObject.GetScene<SplashRoot>(splashScene).Wait())
                .Then(_ => Scene.LoadScenePromise<SplashMovieRoot>(splashMoveScene))
                .Then(_ => SceneObject.GetScene<SplashMovieRoot>(splashMoveScene).Wait())
                .Then(_ => Scene.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                .Then(_ => Preloader = Scene.GetSceneRoot<PreloaderRoot>(EScene.Preloader))
                .Then(_ => Preloader.LoadLoadingScreenPromise(preloaders))
                .Then(_ => Preloader.FadeInLoadingScreenPromise())
                .Then(_ => Scene.LoadScenePromise<PopupCollectionRoot>(EScene.PopupCollection))
                .Then(_ => Scene.LoadScenePromise<ServicesRoot>(EScene.Services))
                .Then(_ => Scene.LoadScenePromise<SceneObject>(titleScene))
                .Then(_ => RegisterListeners())
                .Then(_ => RegisterResolvers())
                .Then(_ => Preloader.FadeOutLoadingScreenPromise());
        }

        private void RegisterListeners()
        {
        }

        private void RegisterResolvers()
        {
            QuerySystem.RegisterResolver(PRELOADER_RESOLVER, RegisterPreloader);
        }
        
        private void RegisterPreloader(IQueryRequest request, IMutableQueryResult result)
        {
            result.Set(Preloader);
        }
    }
}