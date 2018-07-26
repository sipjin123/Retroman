using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.GraphQL
{

    public enum Status
    {
        SUCCESS,
        ERROR,
    }

    public struct GraphResult
    {
        public Status Status;
        public string RawResult;
        public ResultData Result;
        public object Data;
        public T GetData<T>()
        {
            return (T)Data;
        }

        public GraphResult(string result)
        {
            RawResult = result;
            Result = JsonUtility.FromJson<ResultData>(RawResult);
            Data = (object)Result;
            //Debug.LogErrorFormat("[DEBUG] Parse:{0}\n", JsonUtility.ToJson(data));

            if (Result.errors != null && Result.errors.Count > 0)
            {
                Status = Status.ERROR;

                CatchError(Result.errors, delegate (string message, string request)
                {
                    Debug.LogErrorFormat(D.ERROR + "Request:{0} Message:{1}\n", request, message);
                });
            }
            else
            {
                Status = Status.SUCCESS;
            }
        }

        private void CatchError(List<Error> errors, Action<string, string> visit)
        {
            errors.ForEach(e => visit(e.message, e.path.FirstOrDefault()));
        }
    }

    public class GraphRequest
    {
        public GraphInfo Info { get; private set; }

        public GraphRequest() : this(null)
        {
        }

        public GraphRequest(GraphInfo info)
        {
            UpdateInfo(info);
        }

        public void UpdateInfo(GraphInfo info)
        {
            Info = info;
        }

        public virtual void Request(string graphArgs, Action<GraphResult> parser, object graphData = null)
        {
            Debug.LogFormat(D.L("[REQUEST]") + " GraphInfo::Request Args:{0}\n", graphArgs);

            // Convert arguments to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(graphArgs);

            ObservableWWW.Post(Info.GraphURL, bytes)
                .Take(1)
                .Subscribe(_ =>
                {
                    Debug.LogFormat(D.L("[REQUEST]") + " GraphInfo::Request Result:{0}\n", _);
                    GraphResult result = new GraphResult(_);
                    result.Data = graphData;
                    parser(result);
                },
                _ =>
                {
                    Debug.LogErrorFormat(D.E("[REQUEST]") + " GraphInfo::Request Result:{0}:{1}\n", _, _.InnerException);
                    GraphResult result = new GraphResult() { Status = Status.ERROR, Result = null, RawResult = _.Message, Data = graphData };
                    parser(result);
                });
        }
    }
}
