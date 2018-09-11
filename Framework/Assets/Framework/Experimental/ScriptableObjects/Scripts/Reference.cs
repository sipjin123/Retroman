using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    public abstract class Reference<T>
    {
        #region Static Methods

        public static implicit operator T(Reference<T> reference)
        {
            return reference.CurrentValue;
        }

        #endregion Static Methods

        #region Ctor

        public Reference()
        {
        }

        public Reference(T constantValue) : this()
        {
            _constantValue = constantValue;
            _useConstant = true;
        }

        #endregion Ctor

        #region Fields

        [SerializeField, HideInInspector]
        private T _constantValue = default(T);

        [SerializeField, HideInInspector]
        private bool _useConstant = true;

        #endregion Fields

        #region Properties

        [ShowInInspector, PropertyOrder(2), ShowIf("UseConstant")]
        public T ConstantValue
        {
            get { return _constantValue; }
            set { _constantValue = value; }
        }

        [ShowInInspector, PropertyOrder(1)]
        public bool UseConstant
        {
            get { return _useConstant; }
            set { _useConstant = value; }
        }

        public T CurrentValue
        {
            get
            {
                return _useConstant
                    ? _constantValue
                    : Variable != null
                        ? Variable.CurrentValue
                        : default(T);
            }
        }

        [ShowInInspector, InlineEditor, PropertyOrder(2), HideIf("UseConstant")]
        public abstract Variable<T> Variable { get; set; }

        #endregion Properties

        #region Methods

        public IObservable<T> AsObservable()
        {
            Assertion.Assert(_useConstant
                || Variable != null,
                string.Format("{0}<{1}> is not assigned.",
                "Variable",
                "T"));

            return _useConstant
                ? Observable.Return(_constantValue)
                : Variable.AsObservable();
        }

        #endregion Methods
    }
}