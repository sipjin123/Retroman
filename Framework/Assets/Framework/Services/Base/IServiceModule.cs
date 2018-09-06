#if USING_UNIRX

using UniRx;

#endif

namespace Framework.Services
{
    /// <summary>
    /// An interface where all service modules should implement from.
    /// </summary>
    /// <seealso cref="Framework.Services.IService"/>
    public interface IServiceModule
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
        /// Gets the state of the reactive.
        /// </summary>
        /// <value>
        /// The state of the reactive.
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

        #endregion Methods
    }
}