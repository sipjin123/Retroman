using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;

namespace Framework
{
    public class ConcreteInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        protected Scene Scene;

        public virtual void Add()
        {

        }

        public virtual void Install()
        {
            Scene = Scene.GetScene<SystemRoot>(EScene.System);
        }

        public virtual void UnInstall()
        {

        }
    }
}