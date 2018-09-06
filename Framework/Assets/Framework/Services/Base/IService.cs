using System.Collections;

#if USING_UNIRX

using UniRx;

#endif

namespace Framework.Services
{
    /// <summary>
    /// An interface where all services should implement from.
    /// </summary>
    public interface IService
    {
        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        bool IsRequired { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        bool IsReady { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        EServiceState State { get; }

#if USING_UNIRX

        /// <summary>
        /// Gets the reactive state.
        /// </summary>
        /// <value>
        /// The reactive state.
        /// </value>
        ReactiveProperty<EServiceState> ReactiveState { get; }

#endif

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        void Terminate();

        /// <summary>
        /// Initializes this instance as a coroutine.
        /// </summary>
        /// <returns></returns>
        IEnumerator InitializeAsCoroutine();

        /// <summary>
        /// Terminates this instance as a coroutine.
        /// </summary>
        /// <returns></returns>
        IEnumerator TerminateAsCoroutine();

        #endregion Methods
    }
}