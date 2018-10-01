using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    public abstract class Variable<T> : ScriptableObject
    {
        #region Constants

        protected const string MENUITEM_BASE = "Scriptable Variables/";
        protected const int ORDER_BASE = 14000;

        #endregion

        #region Static Methods

        public static implicit operator T(Variable<T> variable)
        {
            if (variable == null)
                return default(T);

            return variable.CurrentValue;
        }

        #endregion Static Methods

        #region Fields

        [SerializeField, HideInInspector]
        private T _currentValue;

        [SerializeField, HideInInspector]
        private T _defaultValue;

        [NonSerialized]
        private BehaviorSubject<T> _publisher;

        #endregion Fields

        #region Properties

        [ShowInInspector, PropertyOrder(1)]
        public T CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_currentValue, value))
                {
                    _currentValue = value;

                    if (_publisher != null)
                        _publisher.OnNext(value);
                }
            }
        }

        [ShowInInspector, PropertyOrder(0)]
        public T DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        #endregion Properties

        #region Methods

        public UniRx.IObservable<T> AsObservable()
        {
            if (_publisher == null)
                _publisher = new BehaviorSubject<T>(_currentValue);

            return _publisher;
        }

        protected void Deinit()
        {

        }

        protected void Init()
        {
            if (CurrentValue == null)
                CurrentValue = DefaultValue;
        }

        public void SetValue(Variable<T> value)
        {
            CurrentValue = value.CurrentValue;
        }

        #endregion Methods

        #region Unity Life Cycle

        protected virtual void OnEnable()
        {
            Init();
        }

        protected virtual void OnDisable()
        {
            Deinit();
        }

        #endregion Unity Life Cycle
    }
}