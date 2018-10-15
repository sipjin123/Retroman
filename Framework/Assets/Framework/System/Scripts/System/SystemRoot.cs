using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Sirenix.OdinInspector;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Sandbox.ButtonSandbox;

namespace Framework
{
    using Sandbox.Services;
    
    public class SystemRoot : Scene
    {
        [SerializeField]
        [TabGroup("New Group", "System")]
        private Camera _SystemCamera;
        public Camera SystemCamera
        {
            get
            {
                return _SystemCamera;
            }
            private set
            {
                _SystemCamera = value;
            }
        }

        [SerializeField]
        [TabGroup("New Group", "System")]
        private GameObject _BlackPanel, _LoadPanel;

        [SerializeField]
        private SystemVersion _SystemVersion;
        public SystemVersion SystemVersion
        {
            
            get
            {
                return _SystemVersion;
            }
            private set
            {
                _SystemVersion = value;
            }
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



        }

        protected override void OnDestroy()
        {
            QuerySystem.RemoveResolver(QueryIds.SystemCamera);
        }

        #endregion

        public void ToggleBlackPanel(bool trigger)
        {
            _BlackPanel.SetActive(trigger);
            Debug.LogError("Panel Black :: "+trigger);
        }
        public void ToggleLoadPanel(bool trigger)
        {
            _LoadPanel.SetActive(trigger);
            Debug.LogError("Panel Load:: " + trigger);
        }

        #region Test
        [SerializeField]
        [TabGroup("New Group", "Test")]
        private Color Color = Color.white;

        [SerializeField]
        [TabGroup("New Group", "Test")]
        private string HexColor = "#C8C500";

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void UpdateColor()
        {
            ColorUtility.TryParseHtmlString(HexColor, out Color);
        }

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void LoadScene()
        {
            LoadSceneAdditivePromise("Shooting");
        }

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void LoadFrameworkScene()
        {
            LoadSceneAdditivePromise<ServicesRoot>(EScene.Services);
        }
        
        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void UnloadScene()
        {
            UnloadScenePromise("Map");
        }

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void UnloadFrameworkScene()
        {
            UnloadScenePromise(EScene.Services);
        }

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Test")]
        public void UnloadScenes()
        {
            StartCoroutine(UnloadAllScenes());
        }
        #endregion
    }
}