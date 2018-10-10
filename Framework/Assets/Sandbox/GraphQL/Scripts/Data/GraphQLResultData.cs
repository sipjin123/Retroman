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
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

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
        LOBBIES_QUERY,
        LOBBY_LEAVE,

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
        GET_WALLET,
        GET_CURRENCY,
        SEND_CURRENCY,
        CONVERT_CURRENCY,
    }

    public enum AdType
    {
        image,
        video,
    }
    #endregion

    #region Graph Result Data
    [Serializable]
    public class Data
    {
        [JProp("player_login")] public PlayerLogin PlayerLogin;//player_login 
        [JProp("player_update")] public PlayerUpdate PlayerUpdate;//player_update
        [JProp("configuration")] public List<Config> Configs;//configuration
        [JProp("announcements")] public List<EventAnnouncement> Announcements;//announcements
        [JProp("lobby_activeInstance")] public LobbyActiveInstance ActiveLobby;//lobby_activeInstance 
        [JProp("lobby_open")] public LobbyInfo OpenedLobby;//lobby_open
        [JProp("lobby_join")] public LobbyJoinReconnect JoinedLobby;//lobby_join
        [JProp("lobby_reconnect")] public LobbyJoinReconnect ReconnectedLobby;//lobby_reconnect
        [JProp("advertisements")] public List<Advertisement> Ads;//advertisements
        [JProp("advertisement_random")] public AdvertisementRandom RandomAd;//advertisement_random
        [JProp("advertisement_end")] public AdvertisementEnd EndAd;//advertisement_end
        [JProp("advertisement_play")] public AdvertisementPlay PlayAd;//adverisement_play
        [JProp("player_getStat")] public PlayerProfileContainer PlayerProfile;//player_profile
        [JProp("event_status")] public GameEventStatus GameEventStatus;//event_status
        [JProp("update_profile")] public UpdateProfileResult ProfileUpdate; //update_profile
        [JProp("player")] public PlayerIDContainer PlayerInfo; //update_profile
        [JProp("leaderboard_standing")] public List<LeaderboardStanding> LeaderboardStandings; //around_leaderboard
        [JProp("leaderboard_players")] public List<LeaderboardStanding> LeadboardPlayers; //around_leaderboard
        [JProp("event_join")] public JoinEventResultData JoinEventResult; //join_event
        [JProp("fgc_wallet")] public FGCWallet FGCWallet; //
        [JProp("genericWallet")] public GenericWallet Currencies;
        [JProp("wallet")] public FGCCurrency Wallet; //wallet
        [JProp("wallet_convert")] public WalletConvert Convert; //wallet_convert
    }

    [Serializable]
    public class Error
    {
        [JProp("message")] public string Message;
        [JProp("locations")] public List<Location> Locations;
        [JProp("path")] public List<string> Path;
    }

    [Serializable]
    public class Location
    {
        [JProp("line")] public int Line;
        [JProp("column")] public int Column;
    }

    [Serializable]
    public class ResultData
    {
        [JProp("data")] public Data Data;
        [JProp("error")] public Error Error;
        [JProp("errors")] public List<Error> Errors;
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