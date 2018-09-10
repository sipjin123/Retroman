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

namespace Sandbox.Registration
{
#if UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS
    public class DatePicker : IDatePicker
    {
        private MessageBroker _Broker;
        public MessageBroker Broker
        {
            get { return _Broker; }
            private set { _Broker = value; }
        }

        public DatePicker(DateTime date, MonoBehaviour behaviour)
        {
            Broker = new MessageBroker();
        }

        public void Select()
        {

        }

        public void Result(string date)
        {

        }
    }
#endif
}