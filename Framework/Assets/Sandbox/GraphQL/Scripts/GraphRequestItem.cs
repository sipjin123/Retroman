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
    // Alias
    using IProg = UniRx.IProgress<float>;

    [Serializable]
    public sealed class GraphRequestItem : IProg, IDisposable
    {
        [ShowInInspector]
        public string Url { get; set; } = string.Empty;

        [ShowInInspector]
        public string Args { get; set; } = string.Empty;
        
        [ShowInInspector]
        public object GraphData { get; set; } = null;

        [ShowInInspector]
        public bool HasPosted { get; private set; } = false;

        [ShowInInspector]
        public float Progress { get; private set; } = 0f;
        
        private Action<GraphResult> GraphParser;
        private CompositeDisposable Disposable;

        #region Ctor
        public GraphRequestItem(string url, string args, Action<GraphResult> parser, object graphData = null)
        {
            Url = url;
            Args = args;
            GraphParser = parser;
            GraphData = graphData;
            Disposable = new CompositeDisposable();
        }
        
        public void Dispose()
        {
            Url = string.Empty;
            Args = string.Empty;
            GraphParser = null;
            GraphData = null;
            Disposable.Dispose();
            Disposable = null;
        }
        #endregion

        #region Post Implementation
        public void Post()
        {
            HasPosted = true;

            // Convert arguments to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(Args);

            Debug.LogFormat(D.GRAPHQL + "GraphRequestItem::Request ULSize:{0} Args:{1}\n", bytes.Length, Args);
            
            ObservableWWW.PostWWW(Url, bytes, this)
                .Timeout(TimeSpan.FromSeconds(10))
                .Take(1)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.GRAPHQL + "GraphRequestItem::Result DLSize:{0} Data:{1}\n", _.bytes.Length, _.text);
                    GraphResult result = new GraphResult(_.text);
                    result.Data = GraphData;
                    GraphParser(result);
                    Progress = 1f;
                },
                _ =>
                {
                    Debug.LogErrorFormat(D.ERROR + "GraphRequestItem::Result Error:{0}:{1}\n Request:{2}\n", _, _.InnerException, Args);
                    GraphResult result = new GraphResult() { Status = Status.ERROR, Result = null, RawResult = _.Message, Data = GraphData };
                    GraphParser(result);
                    Progress = 1f;
                })
                .AddTo(Disposable);
        }
        #endregion

        #region IProg Implementation
        public void Report(float value)
        {
            Progress = value;
        }
        #endregion
    }
}
