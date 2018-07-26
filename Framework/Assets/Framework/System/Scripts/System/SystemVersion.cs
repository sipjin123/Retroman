using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Common;
using Common.Signal;

using Common.Query;

namespace Framework
{
    public class SystemVersion : MonoBehaviour
    {
        [SerializeField]
        private string BuildVersion;

        [SerializeField]
        private string ReleaseVersion;

        [SerializeField]
        private Text LabelVersion;

        private void Awake()
        {
            Debug.LogFormat(D.F + "Local Build:{0} Release:{1}\n", BuildVersion, ReleaseVersion);

            Assertion.AssertNotNull(LabelVersion);

            QuerySystem.RegisterResolver(QueryIds.DevelopmentVersion, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(BuildVersion);
            });

            QuerySystem.RegisterResolver(QueryIds.ReleaseVersion, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(ReleaseVersion);
            });

            UpdateLabel();
        }

        private void Start()
        {
            UpdateLabel();
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(QueryIds.DevelopmentVersion);
            QuerySystem.RemoveResolver(QueryIds.ReleaseVersion);
        }

        public void Hide()
        {
            //#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //            // Disable Build Version
            //            this.labelVersion.gameObject.SetActive(true);
            //            this.labelVersion.text = this.buildVersion;
            //#else
            LabelVersion.gameObject.SetActive(false);
            //#endif
        }

        public void UpdateLabel()
        {
#if DEVELOPMENT_BUILD
            LabelVersion.gameObject.SetActive(true);
            LabelVersion.text = BuildVersion;
#endif

#if UNITY_EDITOR
            Canvas.ForceUpdateCanvases();
#endif
        }
    }

}