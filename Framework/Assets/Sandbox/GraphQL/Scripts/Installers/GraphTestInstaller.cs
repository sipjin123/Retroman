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

namespace Sandbox.GraphQL
{
    using Framework;

    using Sandbox.Audio;
    using Sandbox.Popup;
    using Sandbox.Services;

    public class GraphTestInstaller : ConcreteInstaller, IInstaller
    {
        private PopupCollectionRoot PopupCollection;

        public override void Install()
        {
            base.Install();

            Promise.All(Scene.EndFramePromise())
                .Then(_ => Scene.LoadSceneAdditivePromise<PopupCollectionRoot>(EScene.PopupCollection))
                .Then(_ => Scene.LoadSceneAdditivePromise<AudioRoot>(EScene.Audio))
                .Then(_ => RegisterResolvers())
                .Then(_ => Scene.LoadSceneAdditivePromise<ServicesRoot>(EScene.Services));
        }

        private void RegisterResolvers()
        {
            PopupCollection = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection);

            QuerySystem.RegisterResolver(QueryIds.PopupCollection, RegisterPopup);
        }

        private void RegisterPopup(IQueryRequest request, IMutableQueryResult result)
        {
            result.Set(PopupCollection);
        }
    }
}