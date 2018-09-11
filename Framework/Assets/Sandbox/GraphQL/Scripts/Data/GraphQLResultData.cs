using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{
    #region GraphQL Requests
    public enum GraphQLRequestType
    {
        LOGIN,
        CONFIGURE,
        ANNOUNCEMENTS,

        LOBBY_ACTIVE_INSTANCE,
        LOBBY_OPEN,
        LOBBY_JOIN,
        LOBBY_RECONNECT,
        LOBBY_CLOSE,

        PLAY_AD,
        GET_ALL_ADS,
        GET_RANDOM_AD,
        END_AD,
        PLAYER_PROFILE,
        LEADERBOARD_PLAYER_PROFILE,
        EVENTS_LIST,
        ANNOUNCEMENTS_STATUS,
        UPDATE_PROFILE,

        LEADERBOARD_AROUND,
        LEADERBOARD_TOP,
        EVENT_JOIN,
        PLAYER_DATA,
        EVENT_PLAYER_COUNT,

        PLAYER_UPDATE,

        GET_FGC_WALLET,
        GET_CURRENCY_CONVERSION_RATE,
        CONVERT_CURRENCY,
    }
    #endregion

    #region Graph Result Data
    [Serializable]
    public class Data
    {
        public PlayerLogin player_login;//player_login 
        public PlayerUpdate player_update;//player_update
        public List<Config> configuration;//configuration
        public List<EventAnnouncement> announcements;//announcements
        public LobbyActiveInstance lobby_activeInstance;//lobby_activeInstance 
        public LobbyInfo lobby_open;//lobby_open
        public LobbyJoinReconnect lobby_join;//lobby_join
        public LobbyJoinReconnect lobby_reconnect;//lobby_reconnect
        public List<Advertisement> advertisements;//advertisements
        public AdvertisementRandom advertisement_random;//advertisement_random
        public AdvertisementEnd advertisement_end;//advertisement_end
        public AdvertisementPlay advertisement_play;//adverisement_play
        public PlayerProfileContainer player_getStat;//player_profile
        public GameEventStatus event_status;//event_status
        public UpdateProfileResult update_profile; //update_profile
        public PlayerIDContainer player; //update_profile
        public List<LeaderboardStanding> leaderboard_standing; //around_leaderboard
        public List<LeaderboardStanding> leaderboard_players; //around_leaderboard
        public JoinEventResultData event_join; //join_event
        public FGCWallet fgc_wallet;
        public WalletConversion wallet; //wallet
        public WalletConvert wallet_convert; //wallet_convert
    }

    [Serializable]
    public class Error
    {
        public string message;
        public List<Location> locations;
        public List<string> path;
    }
    [Serializable]
    public class Location
    {
        public int line;
        public int column;
    }

    [Serializable]
    public class ResultData
    {
        public Data data;
        public Error error;
        public List<Error> errors;
    }

    public enum AdType
    {
        image,
        video,
    }
    #endregion

    #region Signals
    [Serializable]
    public struct GraphQLRequestSuccessfulSignal
    {
        [HideInInspector]
        public GraphQLRequestType Type;

        [HideInInspector]
        public object Data;

        public T GetData<T>()
        {
            return (T)Data;
        }
    }

    [Serializable]
    public struct GraphQLRequestFailedSignal
    {
        [HideInInspector]
        public GraphQLRequestType Type;

        [HideInInspector]
        public bool HasData;

        [HideInInspector]
        private object _Data;

        public object Data
        {
            set
            {
                _Data = value;
                HasData = true;
            }
        }

        public T GetData<T>()
        {
            return (T)_Data;
        }
    }
    #endregion
}