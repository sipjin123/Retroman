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
    using Sandbox.ButtonSandbox;
    using Sandbox.Services;
    
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

        [SerializeField]
        GameObject _BlackPanel;
        public void DisableBlackPanel()
        {
            _BlackPanel.SetActive(false);
            Debug.LogError("Panel Black Disabled");
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
            
            Install();

            AddButtonHandler(ButtonType.Close, delegate(ButtonClickedSignal signal)
            {

            });

            AddButtonHandler(ButtonType.Close, delegate (ButtonHoveredSignal signal)
            {

            });

            AddButtonHandler(ButtonType.Close, delegate (ButtonUnhoveredSignal signal)
            {

            });

            AddButtonHandler(ButtonType.Close, delegate (ButtonPressedSignal signal)
            {

            });

            AddButtonHandler(ButtonType.Close, delegate (ButtonReleasedSignal signal)
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