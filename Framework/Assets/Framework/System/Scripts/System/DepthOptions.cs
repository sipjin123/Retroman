using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

namespace Framework
{
    public class DepthOption
    {
        public struct OnSelectOptionSignal
        {
            public string Depth { get; set; }
        }

        [ShowInInspector]
        public string Depth;

        private MessageBroker _Broker = new MessageBroker();
        public MessageBroker Broker
        {
            get { return _Broker; }
            private set { _Broker = value; }
        }

        public DepthOption(string depth)
        {
            SetDepth(depth);
        }

        public void SetDepth(string depth)
        {
            Depth = depth;
        }

        [Button(ButtonSizes.Medium)]
        public void Select()
        {
            Broker = Broker ?? new MessageBroker();
            Broker.Publish(new OnSelectOptionSignal() { Depth = Depth });
        }
    }

    public class DepthOptions
    {
        [SerializeField, ShowInInspector]
        private string _Depth;
        public string Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }

        [SerializeField, HideInInspector]
        private ESceneDepth _SceneDepth;
        public ESceneDepth SceneDepth
        {
            get
            {
                if (_SceneDepth.Equals(ESceneDepth.Invalid))
                {
                    _SceneDepth = Depth.ToEnum<ESceneDepth>();
                }

                return _SceneDepth;
            }
            set { _SceneDepth = value; }
        }

        private bool ShowDrop;
        private bool HideDrop;

        [SerializeField, ShowInInspector]
        private bool Edit;

        //[HideIf("ShowDrop"), InspectorOrder(3)]
        public List<DepthOption> Options;

        //[SerializeField, HideInInspector, InspectorDisabled]
        [SerializeField, ShowInInspector]
        private List<string> Depths;

        private CompositeDisposable Disposables;

        public DepthOptions()
        {
            Disposables = new CompositeDisposable();
        }

        ~DepthOptions()
        {
            Disposables.Clear();
        }

        private void PresetValues()
        {
            Depth = string.Empty;
            SceneDepth = ESceneDepth.Invalid;
        }

        public void UpdateDepth(string depth)
        {
            Depth = depth;
            SceneDepth = depth.ToEnum<ESceneDepth>();
            HideDropDown();
        }

        //[Button(25), ShowIf("HideDrop"), InspectorOrder(2)]
        public void ShowDropDown()
        {
            RecreateOptions();

            ShowDrop = true;
            HideDrop = true;
        }

        //[Button(25), HideIf("HideDrop"), InspectorOrder(2)]
        public void HideDropDown()
        {
            ShowDrop = false;
            HideDrop = false;
        }

        //[Button(25), HideIf("Edit")]
        public void Refresh()
        {
            Options = null;
            Depths = null;

            RecreateOptions();
        }

        private void RecreateOptions()
        {
            if (Depths == null || Depths.Count <= 0)
            {
                Depths = Depths ?? new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkSceneDepths.dat"));

                int count = Depths.Count;
                for (int i = 0; i < count; i++)
                {
                    Depths[i] = Depths[i].Split('=')[0].Trim();
                }
            }

            if (Options == null || Options.Count <= 0)
            {
                Disposables.Clear();
                Options = Options ?? new List<DepthOption>();
                Depths.ForEach(c =>
                {
                    DepthOption option = new DepthOption(c);
                    option.Broker.Receive<DepthOption.OnSelectOptionSignal>()
                        .Subscribe(_ => UpdateDepth(_.Depth))
                        .AddTo(Disposables);

                    Options.Add(option);
                });
            }
        }
    }
}