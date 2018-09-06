using System;

namespace Framework.Services
{
    /// <summary>
    /// An interface where the publishing of responses are handled.
    /// </summary>
    /// <typeparam name="OUT">The type of the out.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IResponsePublisher<OUT> : IDisposable
        where OUT : IResponse
    {
        /// <summary>
        /// Gets the pub sub service.
        /// </summary>
        /// <value>
        /// The pub sub service.
        /// </value>
        IPubSubService<OUT> PubSubService { get; }

        #region Methods

        /// <summary>
        /// Publishes the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="response">The response.</param>
        void Publish(string eventName, OUT response);

        /// <summary>
        /// Registers the publish callback.
        /// </summary>
        /// <param name="pubSubService">The pub sub service.</param>
        /// <param name="publishCallback">The publish callback.</param>
        /// <returns></returns>
        bool RegisterPublishCallback(IPubSubService<OUT> pubSubService, Action<string, OUT> publishCallback);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        #endregion
    }
}