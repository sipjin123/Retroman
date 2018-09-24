using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

using Sandbox.GraphQL;
using Sandbox.Popup;

namespace Sandbox.RGC
{
    public class ProfilePopup : PopupWindow, IPopupWindow
    {
        [SerializeField]
        private GameObject Content;

        [SerializeField]
        private List<ISelectable> Selectables;
        
        protected override void Awake()
        {
            base.Awake();

            Assertion.AssertNotNull(Content);

            Selectables = Content.GetComponentsInChildren<ISelectable>().ToList();
        }

        #region Button Events
        public void OnClickedCancel()
        {
            Selectables.ForEach(s => s.ResetData());
        }

        public void OnClickedSave()
        {
            var signal = new GraphQLUpdatePlayerRequestSignal();
            signal.Entries = new List<StringEntry>();

            List<ISelectable> selectables = Selectables.FindAll(s => s.HasChanges());
            selectables.ForEach(s => s.Save());
            selectables.ForEach(s =>
            {
                StorageData data = s.GetData();
                signal.Entries.Add(new StringEntry()
                {
                    Key = data.Type,
                    Value = data.Value,
                });
            });

            this.Publish(signal);
        }
        #endregion
    }
}