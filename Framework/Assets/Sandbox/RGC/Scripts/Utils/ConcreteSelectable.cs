using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;

using Framework;

namespace Sandbox.RGC
{
    public interface ISelectable
    {
        T GetSelectable<T>() where T : Selectable;
        StorageData GetData();
        void SetData<T>(T data);
        void ResetData();
        void Select();
        void Save();
        bool HasChanges();
    }

    public class ConcreteSelectable : MonoBehaviour, ISelectable
    {
        [SerializeField]
        protected Text Info;

        [SerializeField]
        protected Text Placeholder;

        [SerializeField]
        protected StorageData StorageData;

        [SerializeField]
        protected bool _IsModified;
        public bool IsModified
        {
            get { return _IsModified; }
            private set { _IsModified = value; }
        }

        [SerializeField]
        protected Selectable Selectable;

        protected virtual void Start()
        {
            StorageData.Load();
            
            Assertions();
        }

        protected virtual void OnDestroy()
        {
            Assertions();
        }

        protected virtual void Assertions()
        {
            Assertion.AssertNotNull(Info);
            Assertion.AssertNotNull(Placeholder);
            Assertion.AssertNotNull(Selectable);
        }

        protected virtual void Cleanup()
        {
        }

        public virtual void OnValueChanged()
        {
            IsModified = true;
        }

        #region ISelectables
        public virtual T GetSelectable<T>() where T : Selectable
        {
            return (T)Selectable;
        }

        public StorageData GetData()
        {
            return StorageData;
        }

        public virtual void SetData<T>(T data)
        {

        }

        [Button(ButtonSizes.Medium)]
        public virtual void Select()
        {
            Selectable.Select();
        }

        public virtual bool HasChanges()
        {
            return IsModified;
        }
        #endregion
        
        [Button(ButtonSizes.Medium)]
        public virtual void ResetData()
        {
            IsModified = false;
        }
        
        [Button(ButtonSizes.Medium)]
        public virtual void Save()
        {
            StorageData.Save();
        }
    }
}