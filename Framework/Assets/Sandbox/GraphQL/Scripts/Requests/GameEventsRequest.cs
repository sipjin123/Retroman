using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Framework;

namespace Sandbox.GraphQL
{
    using CodeStage.AntiCheat.ObscuredTypes;

    public struct GraphQLAnnouncementRequestSignal
    {
        public ObscuredString Token;
        public ObscuredBool ShowUpcoming;
    }

    ///"id": "9dce93e0-7371-11e8-9607-ed1d276f8a49",
    ///"subject": "Event in QC",
    ///"content": "This is the description for the event in QC",
    ///"display_to": "2018-06-19T07:32:14.493Z",
    ///"display_from": "2018-06-19T07:32:14.492Z",
    ///"data": "{\"address\":\"Quezon City\",\"registration_start\":\"2018-07-02T03:33:28.000Z\",\"registration_end\":\"2018-07-06T03:33:28.000Z\",\"max_registrants\":100}"
    [Serializable]
    public class EventAnnouncement
    {
        public string id;
        public string subject;
        public string content;
        public string display_to;
        public string display_from;
        public string data;
        public AnnouncementData eventData;

        public string code;
    }

    [Serializable]
    public class AnnouncementData
    {
        public string address;
        public string registration_start;
        public string registration_end;
        public string max_registrants;
        public string image_url;
    }

    public class GameEventsRequest : UnitRequest
    {
        [SerializeField, ShowInInspector]
        private List<EventAnnouncement> Announcements;
        
        private ObscuredString Token;

        public override void Initialze(GraphInfo info)
        {
            base.Initialze(info);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                 .Where(_ => _.Type == GraphQLRequestType.LOGIN)
                 .Subscribe(_ => Token = _.GetData<ObscuredString>())
                 .AddTo(this);

            this.Receive<GraphQLAnnouncementRequestSignal>()
                .Subscribe(_ => GetEvents(Token.GetDecrypted(), _.ShowUpcoming))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.ANNOUNCEMENTS)
                .Subscribe(_ => Announcements = _.GetData<List<EventAnnouncement>>())
                .AddTo(this);
        }

        #region Requests
        /// <summary>
        ///query {
        ///    announcements(
        ///        token: "token_from_player_login"
        ///        show_upcoming: true
        ///    )
        ///    {
        ///        id
        ///        subject
        ///        content
        ///        display_from
        ///        display_to
        ///        data
        ///    }
        ///}
        /// </summary>
        /// <param name="unique_id"></param>
        public void GetEvents(string token, bool showUpcoming = true)
        {
            Builder builder = Builder.Query();
            Function func = builder.CreateFunction("announcements");
            func.AddString("token", token);
            func.AddBoolean("show_upcoming", showUpcoming);
            Return ret = builder.CreateReturn("id", "subject", "content", "display_from", "display_to", "data");

            ProcessRequest(GraphInfo, builder.ToString(), GameAnnouncements);
        }
        #endregion

        #region Parsers
        /// <summary>
        /// Sample Result
        ///{
        ///  "data": {
        ///    "announcements": [
        ///      {
        ///        "id": "9dce93e0-7371-11e8-9607-ed1d276f8a49",
        ///        "subject": "Event in QC",
        ///        "content": "This is the description for the event in QC",
        ///        "display_to": "2018-06-19T07:32:14.493Z",
        ///        "display_from": "2018-06-19T07:32:14.492Z",
        ///        "data": "{\"address\":\"Quezon City\",\"registration_start\":\"2018-07-02T03:33:28.000Z\",\"registration_end\":\"2018-07-06T03:33:28.000Z\",\"max_registrants\":100}"
        ///      },
        ///      {
        ///        "id": "7d84d150-737e-11e8-9607-ed1d276f8a49",
        ///        "subject": "Event in Pasig",
        ///        "content": "This is an event in Pasig",
        ///        "display_to": "2018-06-19T06:13:01.712Z",
        ///        "display_from": "2018-06-19T06:13:01.712Z",
        ///        "data": "{\"address\":\"Pasig City\",\"registration_start\":\"2018-07-02T05:05:52.000Z\",\"registration_end\":\"2018-07-06T05:05:52.000Z\",\"max_registrants\":60}"
        ///      }
        ///    ]
        ///  }
        ///}
        /// </summary>
        /// <param name="result"></param>
        public void GameAnnouncements(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS });
            }
            else
            {
                Announcements = result.Result.data.announcements;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS, Data = Announcements });
            }
        }
        #endregion

        #region Debug
        [Button(25)]
        private void GetGameEvents()
        {
            GetEvents(Token.GetDecrypted());
        }
        #endregion
    }
}
