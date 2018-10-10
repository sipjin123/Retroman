// <copyright file="MonoRoutine.cs" company="Synergy88 Digital Inc.">
// Copyright (c) Synergy88 Digital Inc. All Rights Reserved.
// </copyright>
// <author>Elmer Nocon</author>

using Sirenix.OdinInspector;
using System.Collections;
using UniRx;
using UnityEngine;
using uPromise;
using UDebug = UnityEngine.Debug;

namespace Framework.Common.Routines
{
    using System;

    public abstract class MonoRoutine : MonoBehaviour, IRoutine
    {
        #region Constants

        private const string ErrorDestroyWhileRunning = "The MonoBehaviour was destroyed while it was still running.";

        private const string WarningStartWhileRunning = "A start has been requested but the MonoRoutine was already running. Returning the already created promise.";

        private const string WarningStopNotRunning = "A stop has been requested but the MonoRoutine was not running.";

        #endregion Constants

        #region Serialized Fields

        [SerializeField]
        private bool _enableDebugging = false;

        #endregion Serialized Fields

        #region Protected Fields

        protected Deferred DeferredStart;

        protected Deferred DeferredStop;

        #endregion Protected Fields

        #region Properties

        [ShowInInspector, ReadOnly]
        public bool IsRunning { get; protected set; }

        #endregion Properties

        #region Unity Life Cycle

        protected virtual void OnDestroy()
        {
            if (IsRunning)
                DebugLogError(ErrorDestroyWhileRunning);
        }

        #endregion Unity Life Cycle

        #region Methods

        public Promise Run()
        {
            if (IsRunning)
            {
                DebugLogWarning(WarningStartWhileRunning);
                return DeferredStart.Promise;
            }

            IsRunning = true;
            DeferredStart = new Deferred();
            Observable.FromCoroutine(RunAsCoroutine).Subscribe();

            return DeferredStart.Promise;
        }

        public Promise Stop()
        {
            if (!IsRunning)
            {
                DebugLogWarning(WarningStopNotRunning);
                return Promise.All();
            }

            IsRunning = false;
            DeferredStop = new Deferred();

            return DeferredStop.Promise;
        }

        #endregion Methods

        #region Protected Methods

        protected void DebugLog(object message)
        {
            if (_enableDebugging)
                UDebug.Log(message);
        }

        protected void DebugLogError(object message)
        {
            if (_enableDebugging)
                UDebug.LogError(message);
        }

        protected void DebugLogWarning(object message)
        {
            if (_enableDebugging)
                UDebug.LogWarning(message);
        }

        protected virtual void DoStart()
        {
        }

        protected virtual void DoStop()
        {
            DeferredStart?.Resolve();
            DeferredStop?.Resolve();
        }

        protected virtual void OnStarting()
        {
        }

        protected virtual void OnStarted()
        {
        }

        protected virtual void OnStopping()
        {
        }

        protected virtual void OnStopped()
        {
        }

        protected Promise WaitForSecondsRealtimePromise(float time)
        {
            var deferred = new Deferred();

            StartCoroutine(WaitForSecondsRealtime(time, deferred));

            return deferred.Promise;
        }

        protected Promise WaitUntilPromise(Func<bool> condition)
        {
            var deferred = new Deferred();

            StartCoroutine(WaitUntil(condition, deferred));

            return deferred.Promise;
        }

        #endregion Protected Methods

        #region Private Methods

        [Button("Run", ButtonSizes.Large)]
        private void InspectorRun() => Run();

        [Button("Stop", ButtonSizes.Large)]
        private void InspectorStop() => Stop();

        private IEnumerator RunAsCoroutine()
        {
            OnStarting();

            DoStart();

            OnStarted();

            while (IsRunning)
                yield return null;

            OnStopping();

            DoStop();

            OnStopped();
        }

        private IEnumerator WaitForSecondsRealtime(float time, Deferred deferred)
        {
            yield return new WaitForSecondsRealtime(time);

            deferred.Resolve();
        }

        private IEnumerator WaitUntil(Func<bool> condition, Deferred deferred)
        {
            yield return null;

            while (!condition())
            {
                yield return null;
            }

            deferred.Resolve();
        }

        #endregion Private Methods
    }
}