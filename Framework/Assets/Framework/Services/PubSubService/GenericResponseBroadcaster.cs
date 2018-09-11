using System;

namespace Framework.Services.Implementation
{
    /// <summary>
    /// A generic implementation of IResponseBroadcaster.
    /// </summary>
    /// <seealso cref="Framework.Services.IResponseBroadcaster{Framework.Services.IResponse}" />
    public class GenericResponseBroadcaster : IResponseBroadcaster<IResponse>
    {
        #region Properties

        /// <summary>
        /// Gets the on broadcast.
        /// </summary>
        /// <value>
        /// The on broadcast.
        /// </value>
        public Action<IResponse> OnBroadcast { get; protected set; }

        #endregion Properties

        #region IResponseBroadcaster Methods

        /// <summary>
        /// Broadcasts the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        public virtual void Broadcast(IResponse response)
        {
            OnBroadcast(response);
        }

        /// <summary>
        /// Subscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public virtual void Subscribe(Action<IResponse> subscriber)
        {
            OnBroadcast += subscriber;
        }

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public virtual void Unsubscribe(Action<IResponse> subscriber)
        {
            OnBroadcast -= subscriber;
        }

        #endregion IResponseBroadcaster Methods

        #region IDisposable Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public virtual void Dispose()
        {
            OnBroadcast = null;
        }

        #endregion IDisposable Methods
    }
}