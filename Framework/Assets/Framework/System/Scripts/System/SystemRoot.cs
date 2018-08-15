using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;

namespace Framework
{
    using Sandbox.Services;
    using Sandbox.Shooting;

    // alias
    using UColor = UnityEngine.Color;
    using CColor = Framework.Color;
    using Retroman;

    public class SystemRoot : Scene
    {
        [SerializeField]
        [TabGroup("New Group", "System")]
        private Camera _SystemCamera;
        public Camera SystemCamera
        {
            get { return _SystemCamera; }
            private set { _SystemCamera = value; }
        }

        [SerializeField]
        [TabGroup("New Group", "System")]
        private SystemVersion _SystemVersion;
        public SystemVersion SystemVersion
        {
            get { return _SystemVersion; }
            private set { _SystemVersion = value; }
        }
        
        #region Unity Life Cycle

        protected override void Awake()
        {
            // Setup DI Queries
            QuerySystem.RegisterResolver(QueryIds.SystemCamera, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(SystemCamera);
            });
            
            // Assert cache objects
            Assertion.AssertNotNull(SystemCamera);
            Assertion.AssertNotNull(SystemVersion);

            // Call parent's awake
            base.Awake();

            _SystemVersion.Hide();
            MessageBroker.Default.Receive<ShowVersion>().Subscribe(_ => 
            {
                if (_.IfActive)
                    _SystemVersion.Show();
                else
                    _SystemVersion.Hide();

            }).AddTo(this);
            
            Install();

            AddButtonHandler(EButton.Close, delegate(ButtonClickedSignal signal)
            {

            });

            AddButtonHandler(EButton.Close, delegate (ButtonHoveredSignal signal)
            {

            });

            AddButtonHandler(EButton.Close, delegate (ButtonUnhoveredSignal signal)
            {

            });

            AddButtonHandler(EButton.Close, delegate (ButtonPressedSignal signal)
            {

            });

            AddButtonHandler(EButton.Close, delegate (ButtonReleasedSignal signal)
            {

            });
        }

        protected override void OnDestroy()
        {
            QuerySystem.RemoveResolver(QueryIds.SystemCamera);
        }

        #endregion

        #region Test
        [Button(25)]
        [TabGroup("New Group", "System")]
        public void LoadScene()
        {
            LoadSceneAdditivePromise("Shooting");
        }

        [Button(25)]
        [TabGroup("New Group", "System")]
        public void LoadFrameworkScene()
        {
            LoadSceneAdditivePromise<ServicesRoot>(EScene.Services);
        }

        [Button(25)]
        [TabGroup("New Group", "System")]
        public void Unloadcene()
        {
            UnloadScenePromise("Shooting");
        }

        [Button(25)]
        [TabGroup("New Group", "System")]
        public void UnloadFrameworkcene()
        {
            UnloadScenePromise(EScene.Services);
        }

        [Button(25)]
        [TabGroup("New Group", "System")]
        public void UnloadScenes()
        {
            StartCoroutine(UnloadAllScenes());
        }

        #endregion
    }
}