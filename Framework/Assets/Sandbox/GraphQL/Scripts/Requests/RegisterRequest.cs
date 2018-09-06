 using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Advertisements;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;
using Common.Query;

using Framework;

namespace Sandbox.GraphQL
{
    using Sandbox.RGC;

    public struct GraphQLLoginRequestSignal : IRequestSignal
    {
        public string UniqueId;
    }

    public struct GraphQLUpdatePlayerRequestSignal : IRequestSignal
    {
        public List<StringEntry> Entries;
    }

    [Serializable]
    public class PlayerLogin : IJson
    {
        public string token;
    }

    [Serializable]
    public class PlayerUpdate : IJson
    {
        public string id;
        public string address;      //: String  Address (encrpyted)
        public string birthdate;      //: String  Birthdate(encrpyted)
        public string city;      //: String  City(encrpyted)
        public string email;      //: String  Email address(encrpyted)
        public string first_name;      //: String First name(encrpyted)
        public string gender;      //: String Gender(encrpyted)
        public string last_name;      //: String  Last name(encrpyted)
        public string middle_name;      //: String  Middle name(encrpyted)
        public string mobile_number;      //: String  Mobile phone number(encrpyted)
        public string name;      //: String  Name(encrpyted)
        //public string blacklist;      //: Game(List) List of games the player is not allowed access to
        //public string created_at;      //: String  Registration timestamp in ISO8601 format
        //public string device_id;      //: String  Device ID(nullified when a persistent credential is set during login)
        //public string event_transactions;      //: EventTransaction(List) List of events triggered
        //public string facebook_id;      //: String  Facebook account ID
        //public string games;      //: Game(List) List of games the player has played
        //public string invoices;      //: Invoice(List)  List of invoices
        //public string last_login;      //: String  Last login timestamp in ISO8601 format
        //public string packages;      //: Package(List)  List of plans subscribed to
        //public string payments;      //: Payment(List)  List of payments made
        //public string purchases;      //: Purchase(List) List of purchases made
        //public string raffle_transactions;      //: RaffleTransaction(List)    List of raffle entries earned
        //public string updated_at;      //: String

        public PlayerUpdate Decrypt()
        {
            PlayerUpdate data = new PlayerUpdate();
            data.id = id.Decrypt();
            data.address = address.Decrypt();
            data.birthdate = birthdate.Decrypt();
            data.city = city.Decrypt();
            data.email = email.Decrypt();
            data.first_name = first_name.Decrypt();
            data.gender = gender.Decrypt();
            data.last_name = last_name.Decrypt();
            data.middle_name = middle_name.Decrypt();
            data.mobile_number = mobile_number.Decrypt();
            data.name = name.Decrypt();

            return data;
        }
    }

    public interface IRequestEntry
    {
    }

    public class RequestEntry<V> : IRequestEntry
    {
        public RegData Key;
        public string RawValue { get { return Value.ToString(); } }

        public V Value;

        public void AddParam(ref Function func)
        {
            switch (Key)
            {
                case RegData.gender:
                    func.Add(Key.ToString(), RawValue);
                    break;
                case RegData.address:
                case RegData.birthdate:
                case RegData.city:
                case RegData.email:
                case RegData.first_name:
                case RegData.last_name:
                case RegData.middle_name:
                case RegData.mobile_number:
                    func.AddString(Key.ToString(), RawValue);
                    break;
            }
        }
    }

    public class StringEntry : RequestEntry<string>
    {

    }

    public class GenderEntry : RequestEntry<Gender>
    {

    }

    public class RegisterRequest : UnitRequest
    {
        public static readonly string PLAYER_TOKEN = "PlayerToken";
        public static readonly string PLAYER_INFO = "PlayerInfo";

        [SerializeField]
        private string Token;

        [SerializeField]
        private PlayerUpdate PlayerInfo;

        private void Awake()
        {
            QuerySystem.RegisterResolver(PLAYER_TOKEN, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Token);
            });

            QuerySystem.RegisterResolver(PLAYER_INFO, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(PlayerInfo);
            });
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(PLAYER_TOKEN);
            QuerySystem.RemoveResolver(PLAYER_INFO);
        }

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            this.Receive<GraphQLLoginRequestSignal>()
                .Subscribe(_ => Register(_.UniqueId))
                .AddTo(this);

            this.Receive<GraphQLUpdatePlayerRequestSignal>()
                .Subscribe(_ => UpdateProfile(_.Entries))
                .AddTo(this);
        }

        #region Requests
        /// <summary>
        /// mutation {
        ///    player_login(
        ///        type: guest
        ///        credential: "device_id"
        ///        device_id: "abcd1234"
        ///        game_slug: "gnt"
        ///        build: "1.0.0"
        ///    )
        ///    {
        ///        token
        ///            
        ///     }
        ///}
        /// </summary>
        /// <param name="unique_id"></param>
        public void Register(string unique_id)
        {
            string guest = "guest";
            string credentials = unique_id;
            string deviceId = unique_id;
            string gameSlug = GraphInfo.GameSlug;
            string build = GraphInfo.Build;
            string token = "token";

            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("player_login");
            func.Add("type", guest);
            func.AddString("credential", credentials);
            func.AddString("device_id", deviceId);
            func.AddString("game_slug", gameSlug);
            //func.AddQuoted("build", build);
            Return ret = builder.CreateReturn(token);
            string args = builder.ToString();

            ProcessRequest(GraphInfo, builder.ToString(), PlayerLogin);
        }

        public void RegisterFacebook(string unique_id)
        {
            string guest = "facebook";
            string credentials = unique_id;
            string deviceId = unique_id;
            string gameSlug = GraphInfo.GameSlug;
            string build = GraphInfo.Build;
            string token = "token";

            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("player_login");
            func.Add("type", guest);
            func.AddString("credential", credentials);
            func.AddString("device_id", deviceId);
            func.AddString("game_slug", gameSlug);
            //func.AddQuoted("build", build);
            Return ret = builder.CreateReturn(token);
            string args = builder.ToString();

            ProcessRequest(GraphInfo, builder.ToString(), PlayerLogin);
        }


        /// <summary>
        /// Params:
        /// Required
        ///     token               : String
        /// Optional
        ///     address             : string
        ///     birthdate	        : String
        ///     city	            : String
        ///     email	            : String
        ///     first_name	        : String
        ///     gender	            : male female
        ///     last_name	        : String
        ///     middle_name	        : String
        ///     mobile_number	    : String
        ///     name	            : String
        /// Return
        ///     address	            : String  Address (encrpyted)
        ///     birthdate           : String  Birthdate(encrpyted)
        ///     blacklist           : Game(List) List of games the player is not allowed access to
        ///     city                : String  City(encrpyted)
        ///     created_at          : String  Registration timestamp in ISO8601 format
        ///     device_id           : String  Device ID(nullified when a persistent credential is set during login)
        ///     email               : String  Email address(encrpyted)
        ///     event_transactions  : EventTransaction(List) List of events triggered
        ///     facebook_id         : String  Facebook account ID
        ///     first_name          : String First name(encrpyted)
        ///     games               : Game(List) List of games the player has played
        ///     gender              : String Gender(encrpyted)
        ///     invoices            : Invoice(List)  List of invoices
        ///     last_login          : String  Last login timestamp in ISO8601 format
        ///     last_name           : String  Last name(encrpyted)
        ///     middle_name         : String  Middle name(encrpyted)
        ///     mobile_number       : String  Mobile phone number(encrpyted)
        ///     name                : String  Name(encrpyted)
        ///     packages            : Package(List)  List of plans subscribed to
        ///     payments            : Payment(List)  List of payments made
        ///     purchases           : Purchase(List) List of purchases made
        ///     raffle_transactions : RaffleTransaction(List)    List of raffle entries earned
        ///     updated_at          : String
        /// </summary>
        public void UpdateProfile(List<StringEntry> entries)
        {
            string token = "token";

            Builder builder = Builder.Mutation();
            Function func = builder.CreateFunction("player_update");
            func.AddString(token, Token);
            entries.ForEach(e => e.AddParam(ref func));
            
            Return ret = builder.CreateReturn("name");
            ret.Add("address");
            ret.Add("birthdate");
            ret.Add("city");
            ret.Add("email");
            ret.Add("first_name");
            ret.Add("gender");
            ret.Add("last_name");
            ret.Add("middle_name");
            ret.Add("mobile_number");

            string args = builder.ToString();

            ProcessRequest(GraphInfo, builder.ToString(), PlayerUpdate);
        }
        #endregion

        #region Parsers
        /// <summary>
        /// Sample Result
        /// {
        ///   "data": {
        ///     "player_update": {
        ///       "first_name": "xthDeEL7EtUi+MlVTEKlVg=="
        ///     }
        ///   }
        /// }
        /// </summary>
        /// <param name="result"></param>
        public void PlayerLogin(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.LOGIN });
            }
            else
            {
                Token = result.Result.data.player_login.token;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.LOGIN, Data = Token });
            }
        }

        public void PlayerUpdate(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.PLAYER_UPDATE });
            }
            else
            {
                PlayerInfo = result.Result.data.player_update.Decrypt();
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.PLAYER_UPDATE, Data = PlayerInfo });
            }
        }
        #endregion

        #region Debug
        [Button(25)]
        public void Register()
        {
            Register(Platform.DeviceId);
        }
        #endregion
    }
}
