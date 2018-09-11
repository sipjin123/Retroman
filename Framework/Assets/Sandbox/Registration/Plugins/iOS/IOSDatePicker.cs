using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;

using Framework;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Sandbox.Registration
{
#if UNITY_IOS
    public class DatePicker : IDatePicker, IBroker
    {
        [DllImport("__Internal")]
        private static extern void Select(
            string p_title, 
            string p_done, 
            string p_cancel, 
            string p_gameObjectName, 
            string p_methodName,
            int p_year,
            int p_month,
            int p_day);

        public DateTime Date;
        public MessageBroker Broker { get; private set; }

        private MonoBehaviour Behaviour;

        public DatePicker(DateTime date, MonoBehaviour behaviour)
        {
            Date = date;
            Broker = new MessageBroker();
            Behaviour = behaviour;
            
            this.Receive<OnUpdateDate>()
                .Subscribe(_ =>
                {
                    Date = _.Date;
                    Debug.LogErrorFormat("Android::DatePicker::OnUpdateDate Date:{0}\n", Date.ToLongDateString());

                    Broker.Publish(new OnUpdatePicker() { Data = (object)Date });
                })
                .AddTo(Behaviour);
        }
        
        public void Select()
        {
            // show native date picker here
            Select("Birthday", "Done", "Cancel", Behaviour.gameObject.name, "Result", Date.Year, Date.Month, Date.Day);
        }

        public void Result(string date)
        {
            Debug.LogErrorFormat("IOSDatePicker::Result Date:{0}\n", date);
            string[] dateString = date.Split(',');

            Assertion.Assert(dateString.Length == 3);
            int y = dateString[0].ToInt();
            int m = dateString[1].ToInt();
            int d = dateString[2].ToInt();

            this.Publish(new OnUpdateDate()
            {
                Date = new DateTime(y, m, d)
            });
        }
    }
#endif
}