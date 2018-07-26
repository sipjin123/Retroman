using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Retroman
{
    public class DataManagerService : MonoBehaviour {

        MessageBroker _MessageBroker;
        public MessageBroker MessageBroker { get { return _MessageBroker; } }
        void Awake()
        {
            Factory.Register<DataManagerService>(this);

            GameCoins = PlayerPrefs.GetFloat(TotalGold,0);

        }

        public void ResetPlayerPrefs()
        {

            PlayerPrefs.SetInt("BoughtCat", 0);
            PlayerPrefs.SetInt("BoughtUnicorn", 0);
            PlayerPrefs.SetInt("BoughtYoshi", 0);
            PlayerPrefs.SetInt("BoughtSonic", 0);
            PlayerPrefs.SetInt("BoughtDonkey", 0);
            PlayerPrefs.SetInt("CurrentCharacter", 0);
        }

        public void InjectBroker(MessageBroker broker)
        {
            _MessageBroker = broker;


            _MessageBroker.Receive<AddCoin>().Subscribe(_ =>
            {

                UpdateCoin(_.CoinsToAdd);
            }).AddTo(this);
        }

        public float GameCoins;
        private void UpdateCoin(float coins)
        {
            GameCoins += coins;
            PlayerPrefs.SetFloat(TotalGold, GameCoins);
        }
        public float GetTotalCoins()
        {

            return GameCoins;
        }


        public static string CurrentCharacterSelected = "CurrentCharacterSelected";
        public static string TotalGold = "TotalGold";
    }
}