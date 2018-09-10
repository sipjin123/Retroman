using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Common.Fsm;

using Framework;

namespace Sandbox.Registration
{
    using UUI = UnityEngine.UI;

    public struct OnUpdateDate
    {
        public DateTime Date;
    }

    public interface IDatePicker
    {
        MessageBroker Broker { get; }
        void Select();
        void Result(string date);
    }

    public struct OnUpdatePicker
    {
        public object Data;

        public T GetData<T>()
        {
            return (T)Data;
        }
    }

    public class NativeDatePicker : MonoBehaviour, IDatePicker
    {
        public MessageBroker Broker 
        { 
            get { return Picker.Broker; }
        }

        [HideInInspector]
        public IDatePicker Picker { get; private set; }

        private DateTime PickedDate;

        private void Awake()
        {
            Picker = new DatePicker(DateTime.Now, this);
            Picker.Broker
                .Receive<OnUpdatePicker>()
                .Subscribe(_ => 
                {
                    PickedDate = _.GetData<DateTime>();
                    Debug.LogErrorFormat("NativeDatePicker::Date:{0}\n", PickedDate.ToLongDateString());
                }).AddTo(this);
        }

        public void Select()
        {
            Picker.Select();
        }

        public void Result(string date)
        {
            Debug.LogErrorFormat("Native::Result GameObject:{0} Date:{1}\n", gameObject.name, date);
            Picker.Result(date);
        }
    }
}