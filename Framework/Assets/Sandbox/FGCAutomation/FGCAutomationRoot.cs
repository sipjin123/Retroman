using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Framework;
using Sandbox.FGCAutomation.Data;
using Sandbox.FGCAutomation.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UniRx;
using UnityEngine;
using Zenject;
using CancellationToken = System.Threading.CancellationToken;
using IDataWriter = Sandbox.FGCAutomation.Interfaces.IDataWriter;

namespace Sandbox.FGCAutomation
{
    public class FGCAutomationRoot : Scene
    {
        private List<IDisposable> _Disposables;
        private Queue<TaskData> _TaskDataQueue;
        private TaskData _CurrentTaskData;

        private IDataWriter _DataWriter;

        [ShowInInspector]
        private IProcessor _Processor;

        private bool _IsRunning;

        private int _MatchCounter = 0;
        private readonly char _Ps = Path.DirectorySeparatorChar;
        private DateTime _StartTime;

        [Inject]
        private void Inject(IProcessor processor, IDataWriter writer)
        {
            _Processor = processor;
            _DataWriter = writer;
            Debug.LogError($"{D.ERROR} Zenject FGCAutomationRoot!");
        }

        protected override void Start()
        {
            Debug.LogError($"{D.WARNING} FGCAutomationInstaller being installed!");
            _IsRunning = true;
            _Disposables = new List<IDisposable>();
            _TaskDataQueue = new Queue<TaskData>();

            HandleSubscriptions();
            StartAutomation();

        }

        protected override void OnDestroy()
        {
            _IsRunning = false;
            _Disposables.ForEach(disposable =>
            {
                disposable.Dispose();
            });

        }

        [Button(ButtonSizes.Medium)]
        private void DebugStop()
        {
            Debug.Log($"{D.ERROR} Will try to stop at frame {Time.frameCount}");
        }

        private void HandleSubscriptions()
        {
            this.Receive<TrackCurrency>()
                .Subscribe(_ =>
                {
                    Debug.Log($"{D.WARNING} Track currency received!");

                    _.MatchNumber = _MatchCounter;
                    TaskData taskData;
                    taskData.Data = _;

                    _TaskDataQueue.Enqueue(taskData);
                }).AddTo(_Disposables);

            this.Receive<FGCTrackingMatchStart>()
                .Subscribe(_ =>
                {
                    if (_MatchCounter == 0)
                        _StartTime = DateTime.Now;
                    _MatchCounter++;
                }).AddTo(_Disposables);
        }

        private async void StartAutomation()
        {
            await StartAutomationAsync();
        }

        private async Task StartAutomationAsync()
        {
            Debug.Log($"{D.WARNING} AutomationAsync started");

            while (_IsRunning)
            {
                //await new WaitForSeconds(0.05f);
                await new TimeSpan(0, 0, 0, 0, 50);
                if (_TaskDataQueue.Count == 0)
                    continue;

                _CurrentTaskData = _TaskDataQueue.Dequeue();

                _Processor?.Process(_CurrentTaskData);
                WriteCurrentDataToDisk();
            }
        }

        [Button]
        private void WriteCurrentDataToDisk()
        {
            string fileName = $"{_StartTime:yyyyMMddTHH-mmZ}.txt";
            _DataWriter.InitializeWritePath($"{Application.persistentDataPath}{_Ps}FGC{_Ps}");
            _DataWriter.InitializeWriteFilename(fileName);

            SessionData sessionData = _Processor.AggregateAndFinalize();
            sessionData.SessionStart = _StartTime;
            sessionData.SessionEnd = DateTime.Now;
            _DataWriter.WriteToDisk(sessionData);
        }
    }

}

