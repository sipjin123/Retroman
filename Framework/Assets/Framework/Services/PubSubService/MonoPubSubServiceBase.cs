using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Framework.Services.Implementation
{
    /// <summary>
    /// An abstract MonoBehaviour pub-sub service.
    /// </summary>
    public abstract class MonoPubSubServiceBase : MonoBehaviour, IPubSubService<IResponse>
    {
        #region IPubSubService Properties

        /// <summary>
        /// Gets the broadcasters.
        /// </summary>
        /// <value>
        /// The broadcasters.
        /// </value>
        public IDictionary<string, IResponseBroadcaster<IResponse>> Broadcasters { get; protected set; }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        /// <value>
        /// The publishers.
        /// </value>
        public IList<IResponsePublisher<IResponse>> Publishers { get; protected set; }

        #endregion IPubSubService Properties

        #region IPubSubService Methods

        /// <summary>
        /// Registers the publisher.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        public virtual void RegisterPublisher(IResponsePublisher<IResponse> publisher)
        {
            if (Publishers != null &&
                !Publishers.Contains(publisher))
            {
                publisher.RegisterPublishCallback(this,
                    (eventName, response) => Publish(publisher, eventName, response));

                Publishers.Add(publisher);
            }
        }

        /// <summary>
        /// Subscribes to the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="onReceive">The on receive.</param>
        public virtual void Subscribe(string eventName, Action<IResponse> onReceive)
        {
            if (Broadcasters != null)
            {
                if (!Broadcasters.ContainsKey(eventName))
                    Broadcasters.Add(eventName, CreateResponseBroadcaster());

                Broadcasters[eventName].Subscribe(onReceive);
            }
        }

        /// <summary>
        /// Unsubscribes to the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="onReceive">The on receive.</param>
        public virtual void Unsubscribe(string eventName, Action<IResponse> onReceive)
        {
            if (Broadcasters != null)
            {
                if (Broadcasters.ContainsKey(eventName))
                    Broadcasters[eventName].Unsubscribe(onReceive);
            }
        }

        /// <summary>
        /// Unregisters the publisher.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        public virtual void UnregisterPublisher(IResponsePublisher<IResponse> publisher)
        {
            if (Publishers != null &&
                Publishers.Contains(publisher))
            {
                publisher.Clear();

                Publishers.Remove(publisher);
            }
        }

        #endregion IPubSubService Methods

        #region IService Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsRequired { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsReady { get { return State == EServiceState.Running; } }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public virtual EServiceState State
        {
#if USING_UNIRX
            get { return ReactiveState.Value; }
#else
        get; private set;
#endif
        }

#if USING_UNIRX

        /// <summary>
        /// Gets the reactive state.
        /// </summary>
        /// <value>
        /// The reactive state.
        /// </value>
        public virtual ReactiveProperty<EServiceState> ReactiveState { get; private set; }

#endif

        #endregion IService Properties

        #region IService Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            StartCoroutine(InitializeAsCoroutine());
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public virtual void Terminate()
        {
            StartCoroutine(TerminateAsCoroutine());
        }

        /// <summary>
        /// Initializes this instance as a coroutine.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator InitializeAsCoroutine()
        {
#if USING_UNIRX
            ReactiveState.Value = EServiceState.Running;
#else
        State = EServiceState.Running;
#endif

            yield return null;
        }

        /// <summary>
        /// Terminates this instance as a coroutine.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator TerminateAsCoroutine()
        {
            yield return new WaitForEndOfFrame();

#if USING_UNIRX
            ReactiveState.Value = EServiceState.Terminated;
#else
        State = EServiceState.Terminated;
#endif
        }

        #endregion IService Methods

        #region Methods

        /// <summary>
        /// Publishes the specified publisher.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="response">The response.</param>
        protected virtual void Publish(IResponsePublisher<IResponse> publisher, string eventName, IResponse response)
        {
            if (Publishers != null &&
                Publishers.Contains(publisher) &&
                Broadcasters != null &&
                Broadcasters.Count > 0 &&
                Broadcasters.ContainsKey(eventName))
                Broadcasters[eventName].Broadcast(response);
        }

        /// <summary>
        /// Creates the response broadcaster.
        /// </summary>
        /// <returns></returns>
        protected abstract IResponseBroadcaster<IResponse> CreateResponseBroadcaster();

        #endregion Methods
    }
}