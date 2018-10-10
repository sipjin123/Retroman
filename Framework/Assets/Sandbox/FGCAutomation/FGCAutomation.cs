using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
using JetBrains.Annotations;
using Sandbox.FGCAutomation.Data;
using Sandbox.FGCAutomation.Interfaces;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Sandbox.FGCAutomation
{
    public class FGCAutomation : Scene
    {
        private List<IDisposable> _Disposables;
        private Queue<TaskData> _TaskDataQueue;

        [ShowInInspector]
        private TaskData _CurrentTaskData;

        private IDataWriter _DataWriter;

        private IProcessor _Processor;

        private bool _IsRunning;

        [Inject]
        private void Inject(IProcessor processor, IDataWriter writer)
        {
            _Processor = processor;
            _DataWriter = writer;
            Debug.LogError($"{D.ERROR} Zenject!");
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

        private void HandleSubscriptions()
        {
            MessageBroker.Default.Receive<TrackCurrency>()
                .Subscribe(_ =>
                {
                    Debug.Log($"{D.WARNING} Track currency received!");
                    TaskData taskData;
                    taskData.Data = _;

                    _TaskDataQueue.Enqueue(taskData);
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
                await new WaitForSeconds(0.05f);
                if (_TaskDataQueue.Count == 0)
                    continue;

                _CurrentTaskData = _TaskDataQueue.Dequeue();

                _Processor?.Process(_CurrentTaskData);

                
            }
        }
    }
}
