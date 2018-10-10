using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Newtonsoft.Json;

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
            Result = JsonConvert.DeserializeObject<ResultData>(RawResult);
            Data = (object)Result;

            if (Result.Errors != null && Result.Errors.Count > 0)
            {
                Status = Status.ERROR;

                CatchError(Result.Errors, delegate (string message, string request)
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
            errors.ForEach(e => visit(e.Message, e.Path.FirstOrDefault()));
        }
    }
}
