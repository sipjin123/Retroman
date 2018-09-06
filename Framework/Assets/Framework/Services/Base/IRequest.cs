using System;
using System.Collections;
using System.Collections.Generic;

#if USING_UNIRX

using UniRx;

#endif

namespace Framework.Services
{
    /// <summary>
    /// An interface where all requests should implement from.
    /// </summary>
    /// <typeparam name="OUT">The type of the out.</typeparam>
    /// <seealso cref="Framework.Services.IResponse" />
    public interface IRequest<OUT>
        where OUT : IResponse
    {
        #region Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        IDictionary<string, object> Attributes { get; }

        /// <summary>
        /// Gets the request service.
        /// </summary>
        /// <value>
        /// The request service.
        /// </value>
        IRequestService<IRequest<OUT>, OUT> RequestService { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        IRequest<OUT> AddAttribute(string key, object value);

        /// <summary>
        /// Adds the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        IRequest<OUT> AddAttributes(IDictionary<string, object> attributes);

        /// <summary>
        /// Executes this instance using the specified request service.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService);

        /// <summary>
        /// Executes this instance using the specified request service and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService, int timeoutMilliseconds);

        /// <summary>
        /// Executes this instance using the specified request service and on finish callback.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onFinishCallback);

        /// <summary>
        /// Executes this instance using the specified request service, on finish callback, and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onFinishCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, and on error callback.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback);

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, on error callback, and timeout.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes this instance using the specified request service as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService);

        /// <summary>
        /// Executes this instance using the specified request service and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, int timeoutMilliseconds);

        /// <summary>
        /// Executes this instance using the specified request service and on finish callback as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onFinishCallback);

        /// <summary>
        /// Executes this instance using the specified request service, on finish callback, and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onFinishCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, and on error callback as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback);

        /// <summary>
        /// Executes this instance using the specified request service, on success callback, on error callback, and timeout as a coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback, int timeoutMilliseconds);

#if USING_UNIRX

        /// <summary>
        /// Executes this instance using the specified request service, IObservable{OUT}, a timeout, and a cancellation token as coroutine.
        /// </summary>
        /// <param name="requestService">The request service.</param>
        /// <param name="observer">The observer.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IEnumerator ExecuteAsCoroutine(IRequestService<IRequest<OUT>, OUT> requestService, UniRx.IObservable<OUT> observer, int timeoutMilliseconds, CancellationToken cancellationToken);

#endif

        #endregion Methods
    }
}