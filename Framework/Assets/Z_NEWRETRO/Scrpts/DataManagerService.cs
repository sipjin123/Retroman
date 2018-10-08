using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Sirenix.OdinInspector;
using Synergy88;
using Zenject;

namespace Retroman
{
    public class DataManagerService : SerializedMonoBehaviour {

        public List<ShopItemData> ShopItems;

        MessageBroker _MessageBroker;
        public MessageBroker MessageBroker { get { return _MessageBroker; } }

        public int CurrentCharacterSelected ;
        public float HighScore;
        public float Score;

        public bool IFTestMode;
        public bool IfCanBack;

        [SerializeField]
        private SceneContext _SceneContext;

        [SerializeField]
        private AutomatedController _AutomatedCharController;
        [SerializeField]
        private AutomatedSceneFlow _AutomatedSceneFlow;

        [SerializeField]
        private DataAutomation _AutomatedDataController;


        [SerializeField]
        AutomationUI _AutomationUI;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (IfCanBack == false)
                    return;
                SoundControls.Instance._buttonClick.Play();
                _MessageBroker.Publish(new PressBackButtonINIT());
            }
            if(Input.GetKeyDown(KeyCode.X) || Input.touchCount >2)
            {
                IFTestMode = !IFTestMode;
            }
        }
        
        void Awake()
        {
            Input.multiTouchEnabled = false;
            IFTestMode = false;
            Factory.Register<DataManagerService>(this);
            GameCoins = PlayerPrefs.GetFloat(TotalGold_Key,0);
            SaveThisItem(ShopItems[0].ItemNameId);
            CurrentCharacterSelected = PlayerPrefs.GetInt(CurrentCharacterSelected_Key,1);
            HighScore = PlayerPrefs.GetInt(HighScore_Key, 0);
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
            _AutomatedCharController.InjectBroker(_MessageBroker);
            _AutomatedSceneFlow.InjectBroker(_MessageBroker);
            _AutomatedDataController.InjectBroker(_MessageBroker);
            _AutomationUI.InjectBroker(_MessageBroker);
           // _SceneContext.Install();
            //Factory.Get<AutomatedTestService>().InjectBroker(_MessageBroker);
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
        public float GetHighScore()
        {
            return HighScore;
        }
        public float GetScore()
        {
            return Score;
        }
        public void SetScore(float score)
        {
            Score = score;
        }
        public void SetHighScore(float highScore)
        {
            HighScore = highScore;
            PlayerPrefs.SetInt(HighScore_Key, (int)highScore); 
        }


        [Button]
        void AddCoins()
        {
            PlayerPrefs.SetFloat(TotalGold_Key, 100);
        }

        public static string CurrentCharacterSelected_Key = "CurrentCharacterSelected";
        public static string TotalGold_Key = "TotalGold";
        public static string HighScore_Key = "HighScore";
    }
}