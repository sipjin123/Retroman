using System;
using System.Collections;
using System.Collections.Generic;

#if USING_UNIRX

using UniRx;

#endif

namespace Framework.Services.Implementation
{
    public class GenericRequest : IRequest<IResponse>
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRequest"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public GenericRequest(string id)
        {
            Id = id;
            RequestService = null;
            Attributes = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRequest"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="attributes">The attributes.</param>
        public GenericRequest(string id, IDictionary<string, object> attributes)
            : this(id)
        {
            Attributes = attributes;
        }

        #endregion Ctor

        #region IRequest Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; protected set; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public IDictionary<string, object> Attributes { get; protected set; }

        /// <summary>
        /// Gets the request service.
        /// </summary>
        /// <value>
        /// The request service.
        /// </value>
        public IRequestService<IRequest<IResponse>, IResponse> RequestService { get; protected set; }

        #endregion IRequest Properties

        #region IRequest Methods

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual IRequest<IResponse> AddAttribute(string key, object value)
        {
            if (!Attributes.ContainsKey(key))
                Attributes.Add(key, value);
            else
                Attributes[key] = value;

            return this;
        }

        /// <summary>
        /// Adds the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public virtual IRequest<IResponse> AddAttributes(IDictionary<string, object> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (!Attributes.ContainsKey(attribute.Key))
                    Attributes.Add(attribute);
                else
                    Attributes[attribute.Key] = attribute.Value;
            }

            return this;
        }

        /// <summary>
        /// Executes this instance using the specified request service.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService)
        {
            RequestService = requestService;

            RequestService.Execute(this);
        }

        /// <summary>
        /// Executes this instance using the specified request service and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService, int timeoutMilliseconds)
        {
            RequestService = requestService;

            RequestService.Execute(this, timeoutMilliseconds);
        }

        /// <summary>
        /// Executes this instance using the specified request service and on finish callback.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onFinishCallback)
        {
            RequestService = requestService;

            RequestService.Execute(this, onFinishCallback);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on finish callback, and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onFinishCallback, int timeoutMilliseconds)
        {
            RequestService = requestService;

            RequestService.Execute(this, onFinishCallback, timeoutMilliseconds);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, and on error callback.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onSuccessCallback, Action<IResponse> onErrorCallback)
        {
            RequestService = requestService;

            RequestService.Execute(this, onSuccessCallback, onErrorCallback);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, on error callback, and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        public virtual void Execute(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onSuccessCallback, Action<IResponse> onErrorCallback, int timeoutMilliseconds)
        {
            RequestService = requestService;

            RequestService.Execute(this, onSuccessCallback, onErrorCallback, timeoutMilliseconds);
        }

        /// <summary>
        /// Executes this instance using the specified request service as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this);
        }

        /// <summary>
        /// Executes this instance using the specified request service and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, int timeoutMilliseconds)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, timeoutMilliseconds);
        }

        /// <summary>
        /// Executes this instance using the specified request service and on finish callback as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onFinishCallback)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, onFinishCallback);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on finish callback, and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onFinishCallback, int timeoutMilliseconds)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, onFinishCallback, timeoutMilliseconds);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, and on error callback as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onSuccessCallback, Action<IResponse> onErrorCallback)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, onSuccessCallback, onErrorCallback);
        }

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, on error callback, and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, Action<IResponse> onSuccessCallback, Action<IResponse> onErrorCallback, int timeoutMilliseconds)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, onSuccessCallback, onErrorCallback, timeoutMilliseconds);
        }

#if USING_UNIRX

        /// <summary>
        /// Executes this instance using the specified request service, IObservable{OUT}, a timeout, and a cancellation token as coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="observer">The observer.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<IResponse>, IResponse> requestService, UniRx.IObservable<IResponse> observer, int timeoutMilliseconds, CancellationToken cancellationToken)
        {
            RequestService = requestService;

            return RequestService.ExecuteAsCoroutine(this, observer, timeoutMilliseconds, cancellationToken);
        }

#endif

        #endregion IRequest Methods
    }
}