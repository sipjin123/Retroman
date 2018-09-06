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
        public override void Install()
        {
            base.Install();

            Promise.All(Scene.EndFramePromise())
                .Then(_ => Scene.LoadSceneAdditivePromise<PopupCollectionRoot>(EScene.PopupCollection))
                .Then(_ => Scene.LoadSceneAdditivePromise<AudioRoot>(EScene.Audio))
                .Then(_ => Scene.LoadSceneAdditivePromise<ServicesRoot>(EScene.Services));
        }
    }
}