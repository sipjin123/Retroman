using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Query;
using Common.Utils;

using Framework;

using Sandbox.Services;

namespace Sandbox.Network
{
    public struct OnUpdateInternetConnectionSignal { public bool HasNoInternet; }
    
    public class NETService : BaseService
    {
        public const string HasInternet = "HasInternet";
        public const string GOOGLEURL = "www.google.com";

        [SerializeField]
        private bool HasConnection;

        private TimeSpan TIMEOUT = TimeSpan.FromSeconds(2.5);
        
        public override void InitializeService()
        {
            HasConnection = Application.internetReachability != NetworkReachability.NotReachable;

            CheckInternetConnection();

            RegisterQueries();

            this.Receive<OnPollSignal>()
                .Subscribe(_ => CheckInternetConnection())
                .AddTo(this);

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            HasConnection = Application.internetReachability != NetworkReachability.NotReachable;

            CheckInternetConnection();

            RegisterQueries();

            this.Receive<OnPollSignal>()
                .Subscribe(_ => CheckInternetConnection())
                .AddTo(this);
            
            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        private void CheckInternetConnection()
        {
            ObservableWWW
                .GetWWW(GOOGLEURL)
                .Take(1)
                .Timeout(TIMEOUT)
                .ObserveOnMainThread()
                .Subscribe(
                www =>
                {
                    HasConnection = true;
                    this.Publish(new OnUpdateInternetConnectionSignal() { HasNoInternet = HasConnection });
                },
                error =>
                {
                    HasConnection = Application.internetReachability != NetworkReachability.NotReachable;
                    this.Publish(new OnUpdateInternetConnectionSignal() { HasNoInternet = HasConnection });
                });
        }

        public void RegisterQueries()
        {
            QuerySystem.RegisterResolver(HasInternet, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(HasConnection);
            });
        }
    }
}