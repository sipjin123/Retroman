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
        private GraphRequest Request;

        [SerializeField]
        private List<UnitRequest> Requets = new List<UnitRequest>();
        
        #region Services
        public override void InitializeService()
        {
            InitializeRequests();

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            InitializeRequests();

            yield return null;

            CurrentServiceState.Value = ServiceState.Initialized;
        }

        public override void TerminateService()
        {
        }
        #endregion

        [Button(ButtonSizes.Medium)]
        public void InitializeRequests()
        {
            // Initialize request
            Request.UpdateInfo(GraphInfo);

            // Initialize requets
            Requets = Requets ?? new List<UnitRequest>();
            Requets.Clear();
            Requets.AddRange(GetComponentsInChildren<UnitRequest>());
            Requets.ForEach(r => r.Initialze(GraphInfo, Request));
        }

        [Button(ButtonSizes.Medium)]
        public void Clear()
        {
            Requets.Clear();
        }

        [Button(ButtonSizes.Medium)]
        public void Reload()
        {
            Requets.Clear();
            Requets.AddRange(GetComponentsInChildren<UnitRequest>());
        }
    }
}   