using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{
    [Serializable]
    public class LoginData
    {
        public string Token;
    }

    [Serializable]
    public class LocalAdsData
    {
        public List<Advertisement> Ads;
    }

    [Serializable]
    public class AdsPlayData
    {
        public int InterstitialAds;
        public int RewardAds;
        public DateTime DateOfService;
    }

    [Serializable]
    public class IncompleteAdTransactionData
    {
        public List<PendingTransactions> IncompleteAdTransactions;
    }

    public class GraphQLOfflineData : MonoBehaviour
    {
        private LocalData LoginDataPath;
        private LocalData LocalAdsDataPath;
        private LocalData IncompleteAdTransactionPath;
        private LocalData AdPlayDataPath;

        [SerializeField]
        private LoginData LoginData;

        [SerializeField]
        private LocalAdsData LocalAdsData;

        [SerializeField]
        private IncompleteAdTransactionData PendingTransactions;

        [SerializeField]
        private AdsPlayData _AdsPlayData;
        public AdsPlayData AdsPlayData
        {
            get { return _AdsPlayData; }
            private set { _AdsPlayData = value; }
        }
        
        private void Awake()
        {
            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                .Subscribe(_ => 
                {
                    LoginData.Token = _.GetData<string>();
                    SaveToDisk(LoginDataPath.GetPath(), LoginData);
                })
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.GET_ALL_ADS)
                .Subscribe(_ => 
                {
                    LocalAdsData.Ads = _.GetData<List<Advertisement>>();
                    SaveToDisk(LocalAdsDataPath.GetPath(), LocalAdsData);
                })
                .AddTo(this);
        }
        
        private void InitializePath()
        {
            LoginDataPath = new LocalData(Platform.DeviceId, "loginData.dat");
            LocalAdsDataPath = new LocalData(Platform.DeviceId, "adsData.dat");
            IncompleteAdTransactionPath = new LocalData(Platform.DeviceId, "pendingTransactions.dat");
            AdPlayDataPath = new LocalData(Platform.DeviceId, "adPlayData.dat");
        }

        private void InitializeData()
        {
            if (!File.Exists(LoginDataPath.GetPath()))
            {
                SaveToDisk<LoginData>(LoginDataPath.GetPath(), new LoginData());
            }

            LoginData = LoadFromDisk<LoginData>(LoginDataPath.GetPath()) ?? new LoginData();
            LocalAdsData = LoadFromDisk<LocalAdsData>(LocalAdsDataPath.GetPath()) ?? new LocalAdsData();
            PendingTransactions = LoadFromDisk<IncompleteAdTransactionData>(IncompleteAdTransactionPath.GetPath()) ?? new IncompleteAdTransactionData();
            AdsPlayData = LoadFromDisk<AdsPlayData>(AdPlayDataPath.GetPath()) ?? new AdsPlayData() { DateOfService = DateTime.Now, InterstitialAds = 0, RewardAds = 0 };

            if (PendingTransactions.IncompleteAdTransactions == null)
            {
                PendingTransactions.IncompleteAdTransactions = new List<PendingTransactions>();
            }
        }

        public void Initialize()
        {
            InitializePath();
            InitializeData();
        }

        public void SaveToDisk<T>(string path, T data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, data);
            file.Close();
        }

        public T LoadFromDisk<T>(string path)
        {
            //Assertion.Assert(File.Exists(path), "Missing file! " + path);
            if (!File.Exists(path))
            {
                return default(T);
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                T result= (T)bf.Deserialize(file);
                file.Close();
                return result;
            }
        }

        public string GetUserToken()
        {
            return LoginData.Token;
        }

        public List<Advertisement> GetOldAdList()
        {
            if (LocalAdsData != null)
            {
                return LocalAdsData.Ads;
            }
            else
            {
                return null;
            }
        }

        public List<PendingTransactions> GetPendingTransactionList()
        {
            return PendingTransactions.IncompleteAdTransactions;
        }

        public void SaveFailedTransactions(List<PendingTransactions> Failed)
        {
            PendingTransactions.IncompleteAdTransactions = Failed;
            SaveToDisk<IncompleteAdTransactionData>(IncompleteAdTransactionPath.GetPath(), PendingTransactions);
        }

        public void SaveServicedAds(int interstitial, int reward, DateTime time)
        {
            AdsPlayData.InterstitialAds = interstitial;
            AdsPlayData.RewardAds = reward;
            AdsPlayData.DateOfService = time;
            SaveToDisk<AdsPlayData>(AdPlayDataPath.GetPath(), AdsPlayData);
        }
    }
}