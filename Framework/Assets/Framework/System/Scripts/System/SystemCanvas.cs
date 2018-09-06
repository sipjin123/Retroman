using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using uPromise;

using Common;
using Common.Query;
using Common.Utils;

namespace Framework
{
    [Serializable]
    public class CanvasSetup
    {
        [PropertyRange(1f, 100f)]
        public int PlaneDistance;

        [EnumPaging]
        public RenderMode RenderMode;

        [EnumPaging]
        public ESceneDepth SceneDepth;

        public Canvas Canvas;
    }

    public class SystemCanvas : MonoBehaviour
    {
        [SerializeField]
        private bool UseSystemCamera;
        
        [SerializeField]
        private Camera _Camera;
        public Camera Camera
        {
            get { return _Camera; }
            private set { _Camera = value; }
        }

        [SerializeField]
        private List<CanvasSetup> _CanvasList;
        public List<CanvasSetup> CanvasList
        {
            get { return _CanvasList; }
        }

        public int Count
        {
            get { return CanvasList.Count; }
        }

        public bool IsUsingSystemCamera()
        {
            return UseSystemCamera;
        }
        
        [Button(25)]
        public void SetupSceneCanvas()
        {
            if (UseSystemCamera && QuerySystem.HasResolver(QueryIds.SystemCamera))
            {
                Camera = QuerySystem.Query<Camera>(QueryIds.SystemCamera);
            }

            CanvasList.ForEach(setup => {
                setup.Canvas.renderMode = setup.RenderMode;
                setup.Canvas.planeDistance = setup.PlaneDistance;
                setup.Canvas.sortingOrder = setup.SceneDepth.ToInt();
                setup.Canvas.worldCamera = Camera;

                Debug.LogFormat(D.F + "SystemCanvas::SetupSceneCanvas D:{0} N:{1}\n", setup.PlaneDistance, setup.Canvas.name);
            });
        }

        public void AddCanvas(Canvas canvas)
        {
            CanvasSetup setup = new CanvasSetup();
            setup.RenderMode = RenderMode.ScreenSpaceCamera;
            setup.PlaneDistance = 50;
            setup.SceneDepth = ESceneDepth.Middleground;
            setup.Canvas = canvas;

            AddCanvas(setup);
        }

        public void AddCanvas(CanvasSetup canvasSetup)
        {
            CanvasList.Add(canvasSetup);
        }

        public void AddCanvas(CanvasSetup canvasSetup, int index)
        {
            CanvasList.Insert(index, canvasSetup);
        }

        public void RemoveCanvas(Canvas canvas)
        {
            CanvasList.RemoveAll(s => s.Canvas == null || s.Canvas.Equals(canvas));
        }
    }
}
