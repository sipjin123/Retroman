using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

// alias
using CColor = Framework.Color;

namespace Sandbox.Popup
{
    public class PopupWindow : MonoBehaviour, IPopupWindow
    {
        /// <summary>
        /// Holder for subscriptions to be disposed when the service is terminated.
        /// </summary>
        private CompositeDisposable TerminationDisposables = new CompositeDisposable();

        [SerializeField]
        private PopupData _PopupData = null;
        public PopupData PopupData
        {
            get { return _PopupData; }
            protected set { _PopupData = value; }
        }


        [SerializeField]
        private Canvas _Canvas;
        public Canvas Canvas
        {
            get { return _Canvas; }
            private set { _Canvas = value; }
        }

        [SerializeField]
        private Popup _PopUp;
        public Popup PopUp
        {
            get { return _PopUp; }
            protected set { _PopUp = value; }
        }

        public string Name
        {
            get { return gameObject.name; }
        }

        private string header = CColor.green.LogHeader("[POPUP]");

        protected bool HasPopupData()
        {
            return PopupData != null;
        }

        public void SetPopupData(PopupData data)
        {
            if (data != null)
            {
                _PopupData = data;
            }
        }


        protected virtual void Awake()
        {
            Canvas = gameObject.GetComponent<Canvas>();
            gameObject.name = string.Format("Popup Canvas ({0})", this.PopUp.ToString());
        }

        protected virtual void Start()
        {
            Assertion.Assert(!PopUp.Equals(Popup.Invalid), string.Format(header + " PopupWindow::Awake Invalid Popup! Window:{0}", Name));
        }

        #region Popup Transition

        public Promise In()
        {
            Deferred def = new Deferred();
            StartCoroutine(InAnimation(def));
            return def.Promise;
        }

        public Promise Out()
        {
            Deferred def = new Deferred();
            StartCoroutine(OutAnimation(def));
            return def.Promise;
        }

        private IEnumerator InAnimation(Deferred def)
        {
            yield return null;

            CanvasGroup group = GetComponent<CanvasGroup>();
            float duration = 1.0f;
            float unitValue = 0.0f;

            while (unitValue <= duration)
            {
                unitValue += Time.deltaTime;
                group.alpha = Mathf.Clamp01(unitValue / duration) / 1.0f;
                yield return null;
            }

            group.alpha = 1.0f;
            yield return null;

            def.Resolve();
        }

        private IEnumerator OutAnimation(Deferred def)
        {
            yield return null;

            CanvasGroup group = GetComponent<CanvasGroup>();
            float duration = 1.0f;
            float unitValue = 0.0f;

            while (unitValue <= duration)
            {
                unitValue += Time.deltaTime;
                group.alpha = 1.0f - (Mathf.Clamp01(unitValue / duration) / 1.0f);
                yield return null;
            }

            group.alpha = 0.0f;
            yield return null;

            def.Resolve();
        }

        #endregion
    }
}