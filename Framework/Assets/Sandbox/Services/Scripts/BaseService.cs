using System;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Framework;

namespace Sandbox.Services
{
    using FColor = Framework.Color;

    public class BaseService : MonoBehaviour, IService
    {
        protected virtual void Awake()
        {
        }

        public virtual string ServiceName
        {
            get { return name; }
        }

        [SerializeField]
        protected bool _IsServiceRequired = true;
        public bool IsServiceRequired { get { return _IsServiceRequired; } }

        /// <summary>
        /// True if the service is used as standalone. (e.g. component for a sandbox)
        /// </summary>
        [SerializeField]
        protected bool _DebugEvents = false;
        public virtual bool DebugEvents
        {
            get { return _DebugEvents; }
            protected set { _DebugEvents = value; }
        }

        [SerializeField]
        private ReactiveProperty<ServiceState> _CurrentServiceState = new ReactiveProperty<ServiceState>(ServiceState.Uninitialized);
        public ReactiveProperty<ServiceState> CurrentServiceState { get { return _CurrentServiceState; } }

        public virtual void InitializeService()
        {
            Debug.LogWarningFormat(D.WARNING + " BaseService::InitializeService initialize is not implemented in {0} Service\n", gameObject.name);
        }

        public virtual void TerminateService()
        {
            Debug.LogWarningFormat(D.WARNING + " BaseService::TerminateService initialize is not implemented in {0} Service\n", gameObject.name);
        }
        
        [Button(25)]
        public void InitService()
        {
            InitializeService();
        }

        [Button(25)]
        public void EndService()
        {
            TerminateService();
        }
    }
}
