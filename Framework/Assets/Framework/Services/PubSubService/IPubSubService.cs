using System;
using System.Collections.Generic;

namespace Framework.Services
{
    /// <summary>
    /// An interface where all pub-sub services should implement from.
    /// </summary>
    /// <typeparam name="OUT">The type of the out.</typeparam>
    /// <seealso cref="Framework.Services.IService" />
    public interface IPubSubService<OUT> : IService
        where OUT : IResponse
    {
        #region Properties

        /// <summary>
        /// Gets the broadcasters.
        /// </summary>
        /// <value>
        /// The broadcasters.
        /// </value>
        IDictionary<string, IResponseBroadcaster<OUT>> Broadcasters { get; }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        /// <value>
        /// The publishers.
        /// </value>
        IList<IResponsePublisher<OUT>> Publishers { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the publisher.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        void RegisterPublisher(IResponsePublisher<OUT> publisher);

        /// <summary>
        /// Subscribes to the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="onReceive">The on receive.</param>
        void Subscribe(string eventName, Action<OUT> onReceive);

        /// <summary>
        /// Unsubscribes to the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="onReceive">The on receive.</param>
        void Unsubscribe(string eventName, Action<OUT> onReceive);

        /// <summary>
        /// Unregisters the publisher.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        void UnregisterPublisher(IResponsePublisher<OUT> publisher);

        #endregion Methods
    }
}