using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Common.Query;

using Sandbox.GraphQL;

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

        private void Start()
        {
            Assertion.AssertNotNull(GetRealPrizesButton);
            Assertion.AssertNotNull(GetSynerTix);

            CheckButtons();
        }

        public void CheckButtons()
        {
            string token = QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN);

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
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
