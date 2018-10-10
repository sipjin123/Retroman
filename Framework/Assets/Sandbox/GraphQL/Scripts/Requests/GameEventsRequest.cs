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

using Common;
using Common.Query;

namespace Sandbox.GraphQL
{
    // Alias
    using JProp = Newtonsoft.Json.JsonPropertyAttribute;

    public struct GraphQLAnnouncementRequestSignal : IRequestSignal
    {
        public string Token;
        public bool ShowUpcoming;
    }

    ///"id": "9dce93e0-7371-11e8-9607-ed1d276f8a49",
    ///"subject": "Event in QC",
    ///"content": "This is the description for the event in QC",
    ///"display_to": "2018-06-19T07:32:14.493Z",
    ///"display_from": "2018-06-19T07:32:14.492Z",
    ///"data": "{\"address\":\"Quezon City\",\"registration_start\":\"2018-07-02T03:33:28.000Z\",\"registration_end\":\"2018-07-06T03:33:28.000Z\",\"max_registrants\":100}"
    [Serializable]
    public class EventAnnouncement : IJson
    {
        [JProp("id")] public string Id;
        [JProp("subject")] public string Subject;
        [JProp("content")] public string Content;
        [JProp("display_to")] public string To;
        [JProp("display_from")] public string From;
        [JProp("data")] public string Data;
        [JProp("eventData")] public AnnouncementData Announcement;
        [JProp("code")] public string Code;
    }

    [Serializable]
    public class AnnouncementData : IJson
    {
        public string address;
        public string registration_start;
        public string registration_end;
        public string max_registrants;
        public string image_url;
    }

    public class GameEventsRequest : UnitRequest
    {
        public static readonly string ANNOUNCEMENTS = "Announcements";
        
        [SerializeField]
        private List<EventAnnouncement> Announcements;

        private void Awake()
        {
            QuerySystem.RegisterResolver(ANNOUNCEMENTS, delegate (IQueryRequest request, IMutableQueryResult result) {
                result.Set(Announcements);
            });
        }

        private void OnDestroy()
        {
            QuerySystem.RemoveResolver(ANNOUNCEMENTS);
        }

        public override void Initialze(GraphInfo info, GraphRequest request)
        {
            base.Initialze(info, request);

            this.Receive<GraphQLAnnouncementRequestSignal>()
                .Subscribe(_ => GetEvents(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN), _.ShowUpcoming))
                .AddTo(this);

            this.Receive<GraphQLRequestSuccessfulSignal>()
                .Where(_ => _.Type == GraphQLRequestType.ANNOUNCEMENTS)
                .Subscribe(_ => Announcements = _.GetData<List<EventAnnouncement>>())
                .AddTo(this);
        }

        #region Requests
        private void GetEvents(string token, bool showUpcoming = true)
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
        private void GameAnnouncements(GraphResult result)
        {
            //Assertion.Assert(result.Status == Status.SUCCESS, result.Result);
            if (result.Status == Status.ERROR)
            {
                this.Publish(new GraphQLRequestFailedSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS });
            }
            else
            {
                Announcements = result.Result.Data.Announcements;
                this.Publish(new GraphQLRequestSuccessfulSignal() { Type = GraphQLRequestType.ANNOUNCEMENTS, Data = Announcements });
            }
        }
        #endregion

        #region Debug
        [Button(ButtonSizes.Medium)]
        private void GetGameEvents()
        {
            GetEvents(QuerySystem.Query<string>(RegisterRequest.PLAYER_TOKEN));
        }
        #endregion
    }
}
