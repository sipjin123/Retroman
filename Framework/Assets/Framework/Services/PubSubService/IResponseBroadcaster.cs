using System;

namespace Framework.Services
{
    /// <summary>
    /// An interface where the broadcasting of responses are handled.
    /// </summary>
    /// <typeparam name="OUT">The type of the out.</typeparam>
    public interface IResponseBroadcaster<OUT> : IDisposable
        where OUT : IResponse
    {
        #region Methods

        /// <summary>
        /// Broadcasts the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        void Broadcast(OUT response);

        /// <summary>
        /// Subscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Subscribe(Action<OUT> subscriber);

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe(Action<OUT> subscriber);

        #endregion Methods
    }
}