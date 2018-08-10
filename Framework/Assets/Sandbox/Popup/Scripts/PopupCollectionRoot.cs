using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using uPromise;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.Popup
{
    using UnityEngine;
    
    using Sandbox.UnityAds;

    // alias
    using FScene = Framework.Scene;
    using UScene = UnityEngine.SceneManagement.Scene;
    using UObject = UnityEngine.Object;

    public struct OnShowPopupSignal
    {
        public Popup Popup;
        public PopupData PopupData;
    }

    public class PopupCollectionRoot : FScene
    {
        [SerializeField]
        [TabGroup("New Group", "Popup")]
        private int PlaneDistance = 30;

        [SerializeField]
        [TabGroup("New Group", "Popup")]
        private Popup _CurrentPopup;
        public Popup CurrentPopup
        {
            get { return _CurrentPopup; }
            private set { _CurrentPopup = value; }
        }

        [SerializeField]
        [TabGroup("New Group", "Popup")]
        private GameObject Blocker;

        [SerializeField]
        [TabGroup("New Group", "Popup")]
        private List<PopupWindow> Popups;

        #region Unity Life Cycle

        protected override void Awake()
        {
            base.Awake();

            Assertion.AssertNotNull(Blocker);
            Assertion.AssertNotNull(Popups);

            //All Generic Button Handlers are moved to System Root
            AddButtonHandler(EButton.Popup001, delegate (ButtonClickedSignal signal)
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup001);
                Show(Popup.Popup001);
            });

            AddButtonHandler(EButton.Popup002, delegate (ButtonClickedSignal signal)
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup002);
                Show(Popup.Popup002);
            });

            AddButtonHandler(EButton.Popup003, delegate (ButtonClickedSignal signal)
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup003);
                Show(Popup.Popup003);
            });

            AddButtonHandler(EButton.Close, delegate (ButtonClickedSignal signal)
            {
                //Add Chronos 
                CloseActivePopup();
            });

            this.Receive<OnShowPopupSignal>()
                .Subscribe(_ => Show(_.Popup, _.PopupData))
                .AddTo(this);
        }

        #endregion

        private void CloseActivePopup()
        {
            if (SystemCanvas.Count <= 1)
            {
                return;
            }

            Canvas canvas = SystemCanvas.CanvasList[1].Canvas;
            Popups.Remove(canvas.GetComponent<PopupWindow>());
            SystemCanvas.RemoveCanvas(canvas);

            GameObject.Destroy(canvas.gameObject);

            if (SystemCanvas.Count <= 1)
            {
                Blocker.SetActive(false);
            }

            // Sort
            SortPopupCanvas();
        }

        private void SortPopupCanvas()
        {
            // Fix canvas order
            CanvasSetup canvas;
            int count = SystemCanvas.Count;

            for (int i = 0; i < count; i++)
            {
                canvas = SystemCanvas.CanvasList[i];

                // Back most
                if (i == 0)
                {
                    canvas.PlaneDistance = Mathf.Max(0, PlaneDistance);
                }
                // Front most
                else if (i == 1)
                {
                    canvas.PlaneDistance = Mathf.Max(0, PlaneDistance - 10);
                }
                // Middle
                else
                {
                    canvas.PlaneDistance = Mathf.Max(0, PlaneDistance - 5);
                }

                SystemCanvas.CanvasList[i] = canvas;
                Debug.LogFormat(D.WARNING + " A I:{0} D:{1} N:{2}\n", i, canvas.PlaneDistance, canvas.Canvas.name);
            }
            
            // setup scene canvas
            SystemCanvas.SetupSceneCanvas();
        }

        private IEnumerator Load(Popup popUp, Deferred deferred = null)
        {
            yield return StartCoroutine(Load(popUp, deferred, null));
        }

        private IEnumerator Load(Popup popUp, Deferred deferred = null, PopupData popupData = null)
        {
            string popupScene = popUp.ToString();

            //Debug.LogErrorFormat("++++Popup loaded! P:{0} Def:{1} Time:{2}\n", popUp, deferred, Time.time);

            yield return null;

            AsyncOperation operation = SceneManager.LoadSceneAsync(popupScene, LoadSceneMode.Additive);
            yield return operation;

            //Debug.LogErrorFormat("PopupCollectionRoot::Load Popup:{0} Progress:{1}\n", popupScene, operation.progress);

            Transform root = transform;
            UScene loadedScene = SceneManager.GetSceneByName(popupScene);
            List<GameObject> rawObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<PopupWindow> objects = rawObjects.ToArray<PopupWindow>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(objects.Count == 1);

            // fix object parenting setup
            GameObject obj = objects[0].gameObject;
            obj.transform.SetParent(root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);

            // fix canvas settings
            PopupWindow window = obj.GetComponent<PopupWindow>();
            window.SetPopupData(popupData);
            Popups.Add(window);

            if (obj.GetComponent<Canvas>() != null)
            {
                CanvasSetup setup = new CanvasSetup();
                setup.RenderMode = RenderMode.ScreenSpaceCamera;
                setup.PlaneDistance = PlaneDistance;
                setup.SceneDepth = ESceneDepth.PopUp;
                setup.Canvas = obj.GetComponent<Canvas>();

                if (SystemCanvas.Count <= 1)
                {
                    SystemCanvas.AddCanvas(setup);
                }
                else
                {
                    SystemCanvas.AddCanvas(setup, 1);
                }
            }
            else
            {
                Debug.LogError("The Loaded Object has NO Canvas\n");
            }

            // Fix canvas sorting
            SortPopupCanvas();
            //yield return SceneManager.UnloadSceneAsync(popupScene);
            //yield return SceneManager.UnloadSceneAsync(window.PopUp.ToString());
            Popup pop = window.PopUp;
            yield return StartCoroutine(UnloadScene(pop));

            //Debug.LogErrorFormat("----Popup unloaded! P:{0} Def:{1}\n", pop, deferred);

            if (deferred != null)
            {
                deferred.Resolve();
            }
        }

        private IEnumerator UnloadScene(Popup popup)
        {
            //Debug.LogErrorFormat("UnloadScene:{0} IsValid:{1}\n", popup, SceneManager.GetSceneByName(popup.ToString()).IsValid());

            UScene loadedPopup = SceneManager.GetSceneByName(popup.ToString());

            if (loadedPopup.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(loadedPopup);
            }

            yield return null;

            Resources.UnloadUnusedAssets();

            GC.Collect();
        }

        public bool IsLoaded(Popup popup)
        {
            return Popups.Exists(p => p.PopUp == popup);
        }

        public Promise Show(Popup popup)
        {
            // cache pop up
            return Show(popup, null);
        }

        public Promise Show(Popup popup, PopupData data = null)
        {
            CurrentPopup = popup;
            Blocker.SetActive(true);
            Deferred deferred = new Deferred();
            StartCoroutine(Load(popup, deferred, data));
            return deferred.Promise;
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void Hide()
        {
            CloseActivePopup();
        }

        /// <summary>
        /// Returns the Top/Currently showing Popup
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPopup<T>()
        {
            return Popups[0].GetComponent<T>();
        }

        public bool HasPopUp(Popup popup)
        {
            return Popups.Exists(p => p.PopUp == popup);
        }

        public bool HasActivePopup()
        {
            return Popups.Count >= 1;
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void ShowSamplePopup()
        {
            Show(Popup.Popup001);
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void PlayRewardedAds()
        {
         //   this.Publish(new PlayAdRequestSignal() { IsSkippable = false, CustomAdType = CustomAdType.Reward, FallbackAdType = UnityAds.AdReward.FreeCoins });
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void PlayInterstitialsAds()
        {
          //  this.Publish(new PlayAdRequestSignal() { IsSkippable = true, CustomAdType = CustomAdType.Interstitial, FallbackAdType = UnityAds.AdReward.NoReward });
        }
    }
}