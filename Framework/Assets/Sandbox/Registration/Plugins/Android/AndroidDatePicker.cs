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
#if UNITY_ANDROID
    class DateCallback : AndroidJavaProxy, IBroker
    {
        public DateCallback() : base("android.app.DatePickerDialog$OnDateSetListener")
        {
        }

        private void onDateSet(AndroidJavaObject view, int year, int monthOfYear, int dayOfMonth)
        {
            this.Publish(new OnUpdateDate()
            {
                Date = new DateTime(year, monthOfYear + 1, dayOfMonth)
            });
        }
    }

    public class DatePicker : IDatePicker, IBroker
    {
        public DateTime Date;
        public MessageBroker Broker { get; private set; }

        private AndroidJavaClass UnityPlayer;
        private AndroidJavaObject UnityActivity;

        private long MaxDate = 0L;
        private long MinDate = 0L;

        public DatePicker(DateTime date, MonoBehaviour behaviour)
        {
            Date = date;
            Broker = new MessageBroker();
            
            this.Receive<OnUpdateDate>()
                .Subscribe(_ =>
                {
                    Date = _.Date;
                    Debug.LogErrorFormat("Android::DatePicker::OnUpdateDate Date:{0}\n", Date.ToLongDateString());

                    Broker.Publish(new OnUpdatePicker() { Data = (object)Date });
                })
                .AddTo(behaviour);
        }
        
        public void Select()
        {
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            UnityActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // set min, max date
            DateTime dateMin = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateMax = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // convert date to mills long
            MinDate = dateMin.FromJan1970();
            MaxDate = dateMax.FromJan1970();

            UnityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject datePickerDialog = new AndroidJavaObject("android.app.DatePickerDialog", UnityActivity, new DateCallback(), Date.Year, Date.Month - 1, Date.Day);
                AndroidJavaObject datePicker = datePickerDialog.Call<AndroidJavaObject>("getDatePicker");

                datePicker.Call("setMaxDate", MaxDate);
                datePickerDialog.Call("show");
            }));
        }

        public void Result(string date)
        {
            Debug.LogErrorFormat("AndroidDatePicker::Result Date:{0}\n", date);
        }
    }
#endif
}