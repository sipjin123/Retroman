using System;

namespace Framework.Services.Implementation
{
    /// <summary>
    /// A generic implementation of IResponsePublisher.
    /// </summary>
    /// <seealso cref="Framework.Services.IResponsePublisher{Framework.Services.IResponse}" />
    public class GenericResponsePublisher : IResponsePublisher<IResponse>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the publish callback.
        /// </summary>
        /// <value>
        /// The publish callback.
        /// </value>
        protected Action<string, IResponse> PublishCallback { get; set; }

        #endregion Properties

        #region IResponsePublisher Properties

        /// <summary>
        /// Gets the pub sub service.
        /// </summary>
        /// <value>
        /// The pub sub service.
        /// </value>
        public IPubSubService<IResponse> PubSubService { get; protected set; }

        #endregion IResponsePublisher Properties

        #region IResponsePublisher Methods

        /// <summary>
        /// Publishes the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="response">The response.</param>
        public virtual void Publish(string eventName, IResponse response)
        {
            if (PublishCallback != null)
            {
                PublishCallback(eventName, response);
            }
        }

        /// <summary>
        /// Registers the publish callback.
        /// </summary>
        /// <param name="pubSubService">The pub sub service.</param>
        /// <param name="publishCallback">The publish callback.</param>
        /// <returns></returns>
        public virtual bool RegisterPublishCallback(IPubSubService<IResponse> pubSubService, Action<string, IResponse> publishCallback)
        {
            if (PubSubService != null || pubSubService == null)
                return false;

            PubSubService = pubSubService;

            PublishCallback = publishCallback;

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            PubSubService = null;
            PublishCallback = null;
        }

        #endregion IResponsePublisher Methods

        #region IDisposable Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public virtual void Dispose()
        {
            Clear();
        }

        #endregion IDisposable Methods
    }
}