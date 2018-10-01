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

using UniRx;
namespace Framework
{
    using Common.Utils;
    using Retroman;
    using TMPro;

    public class SystemVersion : MonoBehaviour
    {
        [SerializeField]
        private string BuildVersion;

        [SerializeField]
        private string ReleaseVersion;

        [SerializeField]
        private TextMeshProUGUI LabelVersion;

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


            Factory.Get<DataManagerService>().MessageBroker.Receive<ShowVersion>().Subscribe(_ => 
            {
                if(_.IfActive == false)
                {
                    LabelVersion.gameObject.SetActive(false);

                }
                else
                {
                    LabelVersion.gameObject.SetActive(true);
                }
            }).AddTo(this);

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