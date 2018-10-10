using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Common.Query;
using Framework;
using Sandbox.GraphQL;
using Sandbox.Facebook;
using Sandbox.Popup;
using UniRx;
using OnCloseActivePopup = Sandbox.Popup.OnCloseActivePopup;

namespace Sandbox.RGC
{
    public class FGCResultButton : MonoBehaviour
    {
        /// <summary>
        /// Button to show if the player is not registered to FGC
        /// </summary>
        [SerializeField]
        private GameObject GetRealPrizesButton;

        /// <summary>
        /// Button to show if the player is registered to FGC
        /// </summary>
        [SerializeField]
        private GameObject GetSynerTix;

        [SerializeField]
        private bool _IsSynertix;

        private void Start()
        {
            Assertion.AssertNotNull(GetRealPrizesButton);
            Assertion.AssertNotNull(GetSynerTix);

            CheckButtons();

            this.Receive<OnFacebookLoginSuccessSignal>()
                .Where(_ => _IsSynertix)
                .Subscribe(_ =>
                {
                    OnShowPopupSignal signal;
                    signal.Popup = PopupType.Spinner;
                    signal.PopupData = null;

                    this.Publish(signal);
                });

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.CONFIGURE)
                .Subscribe(_ =>
                {
                    if (_IsSynertix)
                    {
                        GetSynerTix.SetActive(_IsSynertix);
                        GetRealPrizesButton.SetActive(!_IsSynertix);
                    }
                    else
                    {
                        GetSynerTix.SetActive(!_IsSynertix);
                        GetRealPrizesButton.SetActive(_IsSynertix);
                    }

                    OnCloseActivePopup signal;
                    signal.All = true;
                    this.Publish(signal);
                }).AddTo(this);
        }

        public void CheckButtons()
        {
            bool hasFBLogin = QuerySystem.Query<bool>(FBID.HasLoggedInUser);

            if (!hasFBLogin)
            {
                GetRealPrizesButton.SetActive(true);
                GetSynerTix.SetActive(false);
            }
            else
            {
                GetRealPrizesButton.SetActive(false);
                GetSynerTix.SetActive(true);
            }
        }
    }
}
