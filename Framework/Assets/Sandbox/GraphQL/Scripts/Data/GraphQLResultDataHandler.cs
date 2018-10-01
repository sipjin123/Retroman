using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;

using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    public struct UpdateProfileResultSignalSignal : IRequestSignal
    {
        public bool IsUpdated;
    }

    [Serializable]
    public class GraphConfigs
    {
        public static readonly string ENVIRONMENT = "__env";
        public static readonly string BUILD = "__build";
        public static readonly string PUBSUB_ENDPOINT = "pubsub_endpoint";
        public static readonly string PUBLIC_CHANNEL = "pubsub_public_channel";
        public static readonly string PRIVATE_CHANNEL = "pubsub_private_channel";
        public static readonly string SU = "su";
        public static readonly string HOT_ENABLED = "hot_enabled";
        public static readonly string HOT_START = "hot_start";
        public static readonly string HOT_END = "hot_end";
        public static readonly string HOT_MULTIPLIER = "terms_url";
        public static readonly string TERMS_AND_CONDITION = "terms_url";

        private List<Config> Configs;

        public string Environment;
        public string Build;
        public string EndPoint;
        public string PubChannel;
        public string PrivateChannel;
        public string Su;
        public ReactiveProperty<bool> HEnabled = new ReactiveProperty<bool>(false);
        public DateTime HStart;
        public DateTime HEnd;
        public string HMultiplier;
        public string Terms;

        public GraphConfigs(List<Config> configs)
        {
            Configs = configs;

            Func<string, string> GetConfig = delegate (string key)
            {
                if (Configs.Exists(c => c.key.Equals(key)))
                {
                    return Configs.Find(c => c.key.Equals(key)).value;
                }

                return string.Empty;
            };

            Environment = GetConfig(ENVIRONMENT);
            Build = GetConfig(BUILD);
            EndPoint = GetConfig(PUBSUB_ENDPOINT);
            PubChannel = GetConfig(PUBLIC_CHANNEL);
            PrivateChannel = GetConfig(PRIVATE_CHANNEL);
            HEnabled.Value = bool.Parse(GetConfig(HOT_ENABLED));
            Su = GetConfig(SU);
            HStart = ParseTime(GetConfig(HOT_START));
            HEnd = ParseTime(GetConfig(HOT_END));
            HMultiplier = GetConfig(HOT_MULTIPLIER);
            Terms = GetConfig(TERMS_AND_CONDITION);
        }

        public DateTime ParseTime(string time)
        {
            try
            {
                return DateTime.Parse(time);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public double GetSpan()
        {
            return (HEnd - HStart).TotalMilliseconds;
        }
    }
    public class GraphQLResultDataHandler : MonoBehaviour
    {

        public const string RESULT_DATA = "ResultDataStorage";

        private string Token = string.Empty;

        [SerializeField, ShowInInspector]
        private List<Config> Configs;

        [SerializeField, ShowInInspector]
        private List<EventAnnouncement> Announcements;

        private UpdateProfileResult UpdateResult;

        [SerializeField, ShowInInspector]
        private PlayerProfileData profile;

        [SerializeField, ShowInInspector]
        private string playerID;

        [SerializeField, ShowInInspector]
        private Dictionary<string, LeaderboardStanding> LeaderboardData;

        private ReactiveProperty<bool> _IsProfileUpdated = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsProfileUpdated
        {
            get
            {
                return _IsProfileUpdated;
            }
        }

        public string PlayerID
        {
            get { return playerID; }
        }

        public string PlayerName
        {
            get { return profile.first_name; }
        }
        private GraphConfigs _GraphConfigs;
        private CompositeDisposable disp = new CompositeDisposable();

        private void Start()
        {
            QuerySystem.RegisterResolver(RESULT_DATA, delegate (IQueryRequest request, IMutableQueryResult res)
            {
                res.Set(this);
            });

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Subscribe(_ => FilterRequestType(_))
                 .AddTo(this);

        }

        public GraphConfigs GraphConfigs
        {
            get
            {
                return _GraphConfigs;
            }
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(RESULT_DATA);
        }

        public object GetGraphQLData(GraphQLRequestType type)
        {
            switch (type)
            {
                case GraphQLRequestType.LOGIN:
                    return Token;
                case GraphQLRequestType.CONFIGURE:
                    return _GraphConfigs;
                case GraphQLRequestType.ANNOUNCEMENTS:
                    return Announcements;
                case GraphQLRequestType.PLAYER_PROFILE:
                    return profile;
                default:
                    return null;
            }
        }

        private void FilterRequestType(GraphQLRequestSuccessfulSignal result)
        {
            switch (result.Type)
            {
                case GraphQLRequestType.LOGIN:
                    Token = result.GetData<string>();
                    break;

                case GraphQLRequestType.CONFIGURE:
                    ConfigureSuccessResolver(result);
                    break;

                case GraphQLRequestType.ANNOUNCEMENTS:
                    AnnouncementSuccessResolver(result);
                    break;
                case GraphQLRequestType.ANNOUNCEMENTS_STATUS:
                    EventStatusResolver(result);
                    break;

                case GraphQLRequestType.PLAYER_PROFILE:
                    GetPlayerProfileResolver(result);
                    break;

                case GraphQLRequestType.LEADERBOARD_AROUND:
                    EventLeaderboardAroundResolver(result);
                    break;
                case GraphQLRequestType.LEADERBOARD_TOP:
                    EventLeaderboardTopResolver(result);
                    break;
                case GraphQLRequestType.PLAYER_DATA:
                    EventResolvePlayerID(result);
                    break;
            }
        }

        public void ConfigureSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            Configs = result.GetData<List<Config>>();
            _GraphConfigs = new GraphConfigs(Configs);
        }

        public void AnnouncementSuccessResolver(GraphQLRequestSuccessfulSignal result)
        {
            Announcements = result.GetData<List<EventAnnouncement>>();
            foreach (EventAnnouncement announcement in Announcements)
            {
                AnnouncementData data = new AnnouncementData();
                this.Publish(new GetEventPlayersSignal() { announcement_id = announcement.id });
                data = JsonUtility.FromJson<AnnouncementData>(announcement.data);
                announcement.eventData = data;
            }
        }

        public void EventStatusResolver(GraphQLRequestSuccessfulSignal result)
        {
            string json = result.GetData<PlayerProfileContainer>().value;

            json = "{ \"eventsList\": " + json + "}";
            EventsStatus status = JsonUtility.FromJson<EventsStatus>(json);

            if (status.eventsList.Count > 0)
            {
                status.eventsList.ForEach(_item =>
                {
                    Announcements.Find(_event => _event.id == _item.id).code = _item.code;
                });
            }
        }

        public void EventLeaderboardAroundResolver(GraphQLRequestSuccessfulSignal result)
        {
            List<LeaderboardStanding> aroundList = result.GetData<List<LeaderboardStanding>>();
            if (aroundList.Count < 3 && aroundList.Count > 0)
            {
                this.Publish(new LeaderboardDataUpdateRequestSignal());
                return;
            }

            foreach (LeaderboardStanding item in aroundList)
            {
                if (!LeaderboardData.ContainsKey(item.standing))
                {
                    LeaderboardData.Add(item.standing, item);
                }

            }

            StartCoroutine(ProcessLeaderboardData());
        }

        public void EventLeaderboardTopResolver(GraphQLRequestSuccessfulSignal result)
        {
            LeaderboardData.Clear();
            List<LeaderboardStanding> TopList = result.GetData<List<LeaderboardStanding>>();
            foreach (LeaderboardStanding item in TopList)
            {
                LeaderboardData.Add(item.standing, item);
            }
        }

        public void UpdateProfileResolver(GraphQLRequestSuccessfulSignal result)
        {
            UpdateResult = result.GetData<UpdateProfileResult>();
        }

        public void EventResolvePlayerID(GraphQLRequestSuccessfulSignal result)
        {
            playerID = result.GetData<PlayerIDContainer>().id;
        }

        public void GetPlayerProfileResolver(GraphQLRequestSuccessfulSignal result)
        {
            PlayerProfileContainer json = result.GetData<PlayerProfileContainer>();
            profile = new PlayerProfileData();
            profile = JsonUtility.FromJson<PlayerProfileData>(json.value);
            IsProfileUpdated.Value = profile != null;

            PREFS.SetInt("HasLoggedIn", _IsProfileUpdated.Value ? 0 : 1);
        }


        private IEnumerator ProcessLeaderboardData()
        {
            yield return null;
        }

    }
}