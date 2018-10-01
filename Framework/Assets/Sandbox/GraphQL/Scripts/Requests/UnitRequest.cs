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
        
        protected GraphRequest Request = new GraphRequest();

        public virtual void Initialze(GraphInfo info)
        {
            Debug.LogFormat(D.L("[GRAPHQL]") + " Initializing unit graph:{0} Info:{0}\n", gameObject.name, info);

            GraphInfo = info;
        }

        protected virtual void ProcessRequest(GraphInfo info, string arguments, Action<GraphResult> parser, object graphdata = null)
        {
            Request = Request ?? new GraphRequest(info);
            Request.UpdateInfo(info);
            Request.Request(arguments, parser, graphdata);
        }

        protected virtual void CatchError(List<Error> errors, Action<string, string> visit)
        {
            errors.ForEach(e => visit(e.message, e.path.FirstOrDefault()));
        }

        // protected virtual void CatchError<T>(T filter, List<Error> errors, Action<string, string> visit) where T : struct, IConvertible
        // {
        //     Error error = errors.Find(e => e.message.Contains(filter.ToString()));
        //     visit(error.message, error.path.FirstOrDefault());
        // }

        protected virtual bool CatchError<T>(T filter, List<Error> errors, Action<string, string> visit) where T : struct, IConvertible
        {
            Error error = errors.Find(e => e.message.Contains(filter.ToString()));
            if(error != null)
            {
                //error exists in the result
                visit(error.message, error.path.FirstOrDefault());
                return true;
            }
            else
            {
                //error does not exist in the result
                return false;
            }
        }
    }
}
