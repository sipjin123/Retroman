using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{
    using Sandbox.Services;
    
    public class GraphQL : BaseService
    {
        [SerializeField]
        private GraphInfo GraphInfo;

        [SerializeField]
        private List<UnitRequest> Requets = new List<UnitRequest>();

        private GraphRequest Request = new GraphRequest();

        #region Services
        public override void InitializeService()
        {
            InitializeRequests();

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
        }
        #endregion

        [Button(25)]
        public void InitializeRequests()
        {
            // Initialize requets
            Requets = Requets ?? new List<UnitRequest>();
            Requets.Clear();
            Requets.AddRange(GetComponentsInChildren<UnitRequest>());
            Requets.ForEach(r => r.Initialze(GraphInfo));
        }

        [Button(25)]
        public void Clear()
        {
            Requets.Clear();
        }

        [Button(25)]
        public void Reload()
        {
            Requets.Clear();
            Requets.AddRange(GetComponentsInChildren<UnitRequest>());
        }
    }
}   