using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Framework;

namespace Sandbox.Services
{
    public enum LoadType
    {
        Sequential,
        Parallel,
    };

    /// <summary>
    /// This handles initialization and termination of services.
    /// IService references are retrieved from this GameObject's children.
    /// Services are initialized when this MonoBehaviour is enabled.
    /// Services are terminated when this MonoBehaviour is disabled or destroyed.
    /// </summary>
    public class ServicesRoot : Scene
    {
        [SerializeField]
        [TabGroup("New Group", "Services")]
        private LoadType _LoadType = LoadType.Sequential;
        public LoadType LoadType
        {
            get { return _LoadType; }
        }

        [SerializeField]
        [TabGroup("New Group", "Services")]
        private IService[] Services;

        protected override void Start()
        {
            base.Start();

            InitializeServices();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            TerminateServices();
        }

        [Button(ButtonSizes.Medium)]
        [TabGroup("New Group", "Services")]
        public void CacheServices()
        {
            // get all services from child GameObjects
            Services = GetComponentsInChildren<IService>();
        }

        /// <summary>
        /// Finds and initializes services.
        /// </summary>
        private void InitializeServices()
        {
            CacheServices();

            if (Services.Length == 0)
            {
                // let system know services are ready
                this.Publish(new ServicesReadySignal());
                return;
            }

            if (LoadType.Equals(LoadType.Parallel))
            {
                StartCoroutine(InitializeServicesParallelly());
            }
            else
            {
                StartCoroutine(InitializeServicesSequentially());
            }
        }

        private IEnumerator InitializeServicesParallelly()
        {
            yield return null;
            // initialize each service
            foreach (IService service in Services)
            {
                Debug.LogFormat(D.SERVICE + "{0} Initializing {1}...\n", Time.time, service.ServiceName);

                // subscribe to service being ready
                service.CurrentServiceState
                    .Where(state => state > ServiceState.Uninitialized)
                    .Take(1)
                    .Subscribe(state =>
                    {
                        Debug.LogFormat(D.SERVICE + "{0} {1} state: {2}\n", Time.time, service.ServiceName, state);
                        CheckAllServicesReady();
                    })
                    .AddTo(this);

                // initialize service
                service.InitializeService();
            }
        }
        private IEnumerator InitializeServicesSequentially()
        {
            // initialize each service
            foreach (IService service in Services)
            {
                Debug.LogFormat(D.SERVICE + "{0} Initializing {1}...\n", Time.time, service.ServiceName);

                // subscribe to service being ready
                service.CurrentServiceState
                    .Where(state => state > ServiceState.Uninitialized)
                    .Take(1)
                    .Subscribe(state =>
                    {
                        Debug.LogFormat(D.SERVICE + "{0} {1} state: {2}\n", Time.time, service.ServiceName, state);
                        CheckAllServicesReady();
                    })
                    .AddTo(this);

                // initialize service
                yield return StartCoroutine(service.InitializeServiceSequentially());
            }
        }

        /// <summary>
        /// Checks if all services are ready and publishes ServicesReadySignal when they are.
        /// </summary>
        private void CheckAllServicesReady()
        {
            bool allServicesReady =
                Array.Find(Services, s => s.CurrentServiceState.Value == ServiceState.Uninitialized) == null && // check if no service is still at None state
                Array.Find(Services, s => s.CurrentServiceState.Value == ServiceState.Error && s.IsServiceRequired) == null; // check if no required service has an error

            Debug.LogFormat(D.SERVICE + "{0} CheckAllServicesReady: {1}\n", Time.time, allServicesReady);

            if (allServicesReady)
            {
                // let system know services are ready
                this.Publish(new ServicesReadySignal());
            }
        }

        /// <summary>
        /// Terminates all cached services and resets service list.
        /// </summary>
        private void TerminateServices()
        {
            // no need to terminate if there aren't cached services
            if (Services == null) return;

            // terminate services
            foreach (IService service in Services)
            {
                if (service == null)
                {
                    Debug.LogFormat(D.SERVICE + "ServicesRoot: Tried to terminate a null service reference\n");
                    continue;
                }

                service.TerminateService();
            }

            // reset service list
            Services = null;
        }
    }
}
