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
    public interface IRequestSignal
    {

    }

    public class UnitRequest : MonoBehaviour
    {
        [SerializeField]
        protected GraphInfo GraphInfo;

        protected GraphRequest Request;

        public virtual void Initialze(GraphInfo info, GraphRequest request)
        {
            Debug.LogFormat(D.GRAPHQL + " Initializing unit graph:{0} Info:{0}\n", gameObject.name, info);

            GraphInfo = info;
            Request = request;
        }

        protected virtual void ProcessRequest(GraphInfo info, string arguments, Action<GraphResult> parser, object graphdata = null)
        {
            Request.Request(arguments, parser, graphdata);
        }

        protected virtual void ProcessStringRequest(GraphInfo info, string arguments, Action<string> parser, object graphdata = null)
        {
            Request.Request(arguments, parser, graphdata);
        }

        protected virtual void CatchError(List<Error> errors, Action<string, string> visit)
        {
            errors.ForEach(e => visit(e.Message, e.Path.FirstOrDefault()));
        }

        /*
        protected virtual void CatchError<T>(T filter, List<Error> errors, Action<string, string> visit) 
            where T : struct, IConvertible
        {
            Error error = errors.Find(e => e.message.Contains(filter.ToString()));
            visit(error.message, error.path.FirstOrDefault());
        }
        //*/

        protected virtual bool CatchError<T>(T filter, List<Error> errors, Action<string, string> visit) 
            where T : struct, IConvertible
        {
            Error error = errors.Find(e => e.Message.Contains(filter.ToString()));
            if (error != null)
            {
                // Error exists in the result
                visit(error.Message, error.Path.FirstOrDefault());
                return true;
            }

            // Error does not exist in the result
            return false;
        }
    }
}
