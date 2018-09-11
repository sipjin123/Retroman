namespace Framework.Services
{
    /// <summary>
    /// States of IService and IServiceModule.
    /// </summary>
    public enum EServiceState
    {
        /// <summary>
        /// When a service has not been started.
        /// </summary>
        Unitialized,

        /// <summary>
        /// When a service is in the process of starting.
        /// </summary>
        Initializing,

        /// <summary>
        /// When a service is running.
        /// </summary>
        Running,

        /// <summary>
        /// When a service is halted.
        /// </summary>
        Halted,

        /// <summary>
        /// When a service is in the process of resuming.
        /// </summary>
        Resuming,

        /// <summary>
        /// When a service is in the process of terminating.
        /// </summary>
        Terminating,

        /// <summary>
        /// When a service has been terminated.
        /// </summary>
        Terminated
    }
}