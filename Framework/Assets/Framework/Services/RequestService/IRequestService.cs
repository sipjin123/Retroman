using System;
using System.Collections;
using System.Collections.Generic;

#if USING_UNIRX

using UniRx;

#endif

namespace Framework.Services
{
    /// <summary>
    /// An interface where all request services should implement from.
    /// </summary>
    /// <typeparam name="IN">The type of the in.</typeparam>
    /// <typeparam name="OUT">The type of the out.</typeparam>
    /// <seealso cref="Framework.Services.IService" />
    /// <seealso cref="Framework.Services.IRequest{OUT}" />
    /// <seealso cref="Framework.Services.IResponse" />
    public interface IRequestService<IN, OUT> : IService
        where IN : IRequest<OUT>
        where OUT : IResponse
    {
        #region Methods

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <returns></returns>
        IN CreateRequest();

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        IN CreateRequest(IDictionary<string, object> attributes);

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        void Execute(IN request);

        /// <summary>
        /// Executes the specified request with a timeout.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IN request, int timeoutMilliseconds);

        /// <summary>
        /// Executes the specified request with an onFinishCallback.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        void Execute(IN request, Action<OUT> onFinishCallback);

        /// <summary>
        /// Executes the specified request with an onFinishCallback and a timeout.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IN request, Action<OUT> onFinishCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes the specified request with an onSuccessCallback and an onErrorCallback.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        void Execute(IN request, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback);

        /// <summary>
        /// Executes the specified request with an onSuccessCallback, an onErrorCallback, and a timeout.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        void Execute(IN request, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes the specified request as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        IEnumerator ExecuteAsCoroutine(IN request);

        /// <summary>
        /// Executes the specified request with a timeout as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IN request, int timeoutMilliseconds);

        /// <summary>
        /// Executes the specified request with an onFinishCallback as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        IEnumerator ExecuteAsCoroutine(IN request, Action<OUT> onFinishCallback);

        /// <summary>
        /// Executes the specified request with an onFinishCallback and a timeout as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onFinishCallback">The on finish callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IN request, Action<OUT> onFinishCallback, int timeoutMilliseconds);

        /// <summary>
        /// Executes the specified request with an onSuccessCallback and an onErrorCallback as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        IEnumerator ExecuteAsCoroutine(IN request, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback);

        /// <summary>
        /// Executes the specified request with an onSuccessCallback, an onErrorCallback, and a timeout as a coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        IEnumerator ExecuteAsCoroutine(IN request, Action<OUT> onSuccessCallback, Action<OUT> onErrorCallback, int timeoutMilliseconds);

#if USING_UNIRX

        /// <summary>
        /// Executes the specified request with an IObservable{OUT}, a timeout and a cancellationToken as coroutine.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="observer">The observer.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IEnumerator ExecuteAsCoroutine(IN request, UniRx.IObservable<OUT> observer, int timeoutMilliseconds, CancellationToken cancellationToken);

#endif

        #endregion Methods
    }
}