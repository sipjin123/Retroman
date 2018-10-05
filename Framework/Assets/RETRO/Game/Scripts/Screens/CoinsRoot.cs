using UnityEngine;

using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common;
using Common.Signal;
using Common.Utils;
using Common.Query;
using Framework;
using UniRx;
namespace Retroman {

	public class CoinsRoot : Scene
    {
        public Canvas CoinCanvas;
        MessageBroker messsageBroker;
        public Text _CoinsText;
        private void Awake()
        {
            base.Awake();

            messsageBroker = Factory.Get<DataManagerService>().MessageBroker;

            messsageBroker.Receive<ToggleCoins>().Subscribe(_ =>
            {
                Debug.LogError("Toggling Coiins: " + _.IfActive);
                CoinCanvas.enabled = _.IfActive;
                _CoinsText.text = Factory.Get<DataManagerService>().GameCoins.ToString();
            }).AddTo(this);

            messsageBroker.Receive<RefreshCoins>().Subscribe(_ =>
            {
                _CoinsText.text = Factory.Get<DataManagerService>().GameCoins.ToString();
            }).AddTo(this);
            messsageBroker.Receive<LaunchGamePlay>().Subscribe(_ =>
            {
                CoinCanvas.enabled = false;
            }).AddTo(this);
            
        }
    }

}