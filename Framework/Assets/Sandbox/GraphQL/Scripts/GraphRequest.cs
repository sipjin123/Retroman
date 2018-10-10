using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{
    public sealed class GraphRequest : SerializedMonoBehaviour
    {
        public GraphInfo Info { get; private set; }

        [SerializeField]
        private bool CanStack = false;

        [SerializeField]
        private Dictionary<string, bool> FilterMap;

        [SerializeField]
        private List<GraphRequestItem> Requests;

        private void Start()
        {
            FilterMap = FilterMap ?? new Dictionary<string, bool>();
        }

        private void Update()
        {
            if (Requests.Count <= 0)
            {
                return;
            }

            GraphRequestItem request = Requests.FirstOrDefault();
            if (request == null)
            {
                Requests.RemoveAll(r => r == null);
                return;
            }

            if (!request.HasPosted)
            {
                request.Post();
                return;
            }

            if (request.Progress >= 1f)
            {
                FilterMap.Remove(request.Args);
                request.Dispose();
                Requests.RemoveAt(0);
            }
        }

        public void UpdateInfo(GraphInfo info)
        {
            Info = info;
        }

        public void Request(string graphArgs, Action<GraphResult> parser, object graphData = null)
        {
            // TODO: Add filter here.
            if (CanStack || !FilterMap.ContainsKey(graphArgs))
            {
                FilterMap.SafeAdd(graphArgs, true);
                Requests.Add(new GraphRequestItem(Info.GraphURL, graphArgs, parser, graphData));
            }
        }

        /// <summary>
        /// Note: Request with string parser is currenntly excluded to request queueing
        /// </summary>
        /// <param name="graphArgs"></param>
        /// <param name="parser"></param>
        /// <param name="graphData"></param>
        public void Request(string graphArgs, Action<string> parser, object graphData = null)
        {
            Debug.LogFormat(D.L("[REQUEST]") + " GraphInfo::Request<string> Args:{0}\n", graphArgs);

            // Convert arguments to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(graphArgs);

            ObservableWWW.Post(Info.GraphURL, bytes)
                .Take(1)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.L("[REQUEST]") + " GraphInfo::Request Result:{0}\n", _);
                    parser(_);
                },
                _ =>
                {
                    Debug.LogErrorFormat(D.E("[REQUEST]") + " GraphInfo::Request Result:{0}:{1}\n", _, _.InnerException);
                    //GraphResult result = new GraphResult() { Status = Status.ERROR, Result = null, RawResult = _.Message, Data = graphData };
                    parser(_.Message);
                });
        }
    }
}
