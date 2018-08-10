using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Sirenix.OdinInspector;
using Synergy88;

namespace Retroman
{
    public class DataManagerService : SerializedMonoBehaviour {

        public List<ShopItemData> ShopItems;

        MessageBroker _MessageBroker;
        public MessageBroker MessageBroker { get { return _MessageBroker; } }

        public int CurrentCharacterSelected ;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _MessageBroker.Publish(new PressBackButtonINIT());
            }
        }
        
        void Awake()
        {
            Factory.Register<DataManagerService>(this);

            GameCoins = PlayerPrefs.GetFloat(TotalGold_Key,0);
            SaveThisItem(ShopItems[0].ItemNameId);
            CurrentCharacterSelected = PlayerPrefs.GetInt(CurrentCharacterSelected_Key,1);
        }

        public void SaveThisItem(string item)
        {

            PlayerPrefs.SetInt(item, 1);
        }

        public bool DoesThisExist(string item)
        {

            if(PlayerPrefs.GetInt(item,0) == 1)
            return true;
            return false;

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
            PlayerPrefs.SetFloat(TotalGold_Key, GameCoins);
        }

        public void UpdateCurrentCharacter(int currentChar)
        {
            CurrentCharacterSelected = currentChar;
            PlayerPrefs.SetInt(CurrentCharacterSelected_Key, currentChar);
        }
        public int GetCurrentCharacter()
        {
            return PlayerPrefs.GetInt(CurrentCharacterSelected_Key, 1);
        }

        public float GetTotalCoins()
        {

            return GameCoins;
        }
        
        
        [Button]
        void AddCoins()
        {
            PlayerPrefs.SetFloat(TotalGold_Key, 100);
        }

        public static string CurrentCharacterSelected_Key = "CurrentCharacterSelected";
        public static string TotalGold_Key = "TotalGold";
    }
}