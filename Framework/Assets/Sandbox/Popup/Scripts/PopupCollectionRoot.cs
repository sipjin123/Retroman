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

    using Sandbox.GraphQL;
    using Sandbox.UnityAds;

    // alias
    using FScene = Framework.Scene;
    using UScene = UnityEngine.SceneManagement.Scene;
    using UObject = UnityEngine.Object;
    using Sandbox.ButtonSandbox;

    public struct OnShowPopupSignal
    {
        public PopupType Popup;
        public PopupData PopupData;
    }

    public struct OnCloseActivePopup { }

    public partial class PopupCollectionRoot : FScene
    {
        [SerializeField]
        [TabGroup("New Group", "Popup")]
        private int PlaneDistance = 30;
        
        [SerializeField]
        [LabelText("PopupType")]
        [ValueDropdown("GetPopups")]
        [TabGroup("New Group", "Popup")]
        private string _CurrentPopup;
        public int CurrentPopup
        {
            get { return _CurrentPopup.ToPopupValue(); }
            private set { _CurrentPopup = value.ToPopup().Type; }
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
            /*
            AddButtonHandler(ButtonType.Popup001, delegate (ButtonClickedSignal signal)
            {
                Show(PopupType.Popup001);
            });

            AddButtonHandler(ButtonType.Popup002, delegate (ButtonClickedSignal signal)
            {
                Show(PopupType.Popup002);
            });

            AddButtonHandler(ButtonType.Popup003, delegate (ButtonClickedSignal signal)
            {
                Show(PopupType.Popup003);
            });
            //*/

            AddButtonHandler(ButtonType.Close, delegate (ButtonClickedSignal signal)
            {
                //Add Chronos 
                CloseActivePopup();
            });

            this.Receive<OnShowPopupSignal>()
                .Subscribe(_ => Show(_.Popup, _.PopupData))
                .AddTo(this);

            this.Receive<OnCloseActivePopup>()
                .Subscribe(_ => this.CloseActivePopup())
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

        /// <summary>
        /// NOTE: Temp fix for PopupCollection sort order bug
        /// TODO: Adjust SystemCanvas to handle the stack order of popups
        /// </summary>
        private void SortPopupCanvas()
        {
            Camera camera = null;
            bool hasCamera = true;
            if (hasCamera = (SystemCanvas.IsUsingSystemCamera() && QuerySystem.HasResolver(QueryIds.SystemCamera)))
            {
                camera = QuerySystem.Query<Camera>(QueryIds.SystemCamera);
            }

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
                    canvas.Canvas.sortingOrder = canvas.SceneDepth.ToInt() - 2;
                }
                // Front most
                else if (i == 1)
                {
                    canvas.PlaneDistance = Mathf.Max(0, PlaneDistance - 10);
                    canvas.Canvas.sortingOrder = canvas.SceneDepth.ToInt();
                }
                // Middle
                else
                {
                    canvas.PlaneDistance = Mathf.Max(0, PlaneDistance - 5);
                    canvas.Canvas.sortingOrder = canvas.SceneDepth.ToInt() - 1;
                }

                canvas.Canvas.planeDistance = canvas.PlaneDistance;
                canvas.Canvas.renderMode = canvas.RenderMode;

                if (hasCamera)
                {
                    canvas.Canvas.worldCamera = camera;
                }

                SystemCanvas.CanvasList[i] = canvas;
                Debug.LogFormat(D.WARNING + " A I:{0} D:{1} N:{2}\n", i, canvas.PlaneDistance, canvas.Canvas.name);
            }
        }
        
        private IEnumerator Load(PopupType popUp, Deferred deferred = null)
        {
            yield return StartCoroutine(Load(popUp, deferred, null));
        }

        private IEnumerator Load(PopupType popUp, Deferred deferred = null, PopupData popupData = null)
        {
            string popupScene = popUp.Type;

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
            GameObject obj = objects.FirstOrDefault().gameObject;
            obj.transform.SetParent(root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);

            // fix canvas settings
            Canvas canvas = obj.GetComponent<Canvas>();
            PopupWindow window = obj.GetComponent<PopupWindow>();
            window.SetPopupData(popupData);
            Popups.Add(window);
            
            if (canvas != null)
            {
                CanvasSetup setup = new CanvasSetup();
                setup.RenderMode = RenderMode.ScreenSpaceCamera;
                setup.PlaneDistance = PlaneDistance;
                setup.SceneDepth = ESceneDepth.PopUp;
                setup.Canvas = canvas;

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
                Debug.LogErrorFormat(D.ERROR + "PopupCollectionRoot::Load The loaded object:{0} has no Canvas component.\n", obj.name);
            }

            // Fix canvas sorting
            SortPopupCanvas();
            //yield return SceneManager.UnloadSceneAsync(popupScene);
            //yield return SceneManager.UnloadSceneAsync(window.PopUp.ToString());
            PopupType pop = window.PopUp.ToPopup();
            yield return StartCoroutine(UnloadScene(pop));

            //Debug.LogErrorFormat("----Popup unloaded! P:{0} Def:{1}\n", pop, deferred);

            if (deferred != null)
            {
                deferred.Resolve();
            }
        }

        private IEnumerator UnloadScene(PopupType popup)
        {
            //Debug.LogErrorFormat("UnloadScene:{0} IsValid:{1}\n", popup, SceneManager.GetSceneByName(popup.ToString()).IsValid());

            UScene loadedPopup = SceneManager.GetSceneByName(popup.Type);

            if (loadedPopup.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(loadedPopup);
            }

            yield return null;

            Resources.UnloadUnusedAssets();

            GC.Collect();
        }

        public bool IsLoaded(PopupType popup)
        {
            return Popups.Exists(p => p.PopUp == popup.Value);
        }

        public Promise Show(PopupType popup)
        {
            // cache pop up
            return Show(popup, null);
        }

        public Promise Show(PopupType popup, PopupData data = null)
        {
            CurrentPopup = popup.Value;
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

        public bool HasPopUp(PopupType popup)
        {
            return Popups.Exists(p => p.PopUp == popup.Value);
        }

        public bool HasActivePopup()
        {
            return Popups.Count >= 1;
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void ShowSamplePopup()
        {
            Show(PopupType.Popup001);
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void PlayRewardedAds()
        {
            this.Publish(new PlayAdRequestSignal() { IsSkippable = false, CustomAdType = CustomAdType.Reward, FallbackAdType = UnityAds.AdReward.FreeCoins });
        }

        [TabGroup("New Group", "Popup")]
        [Button(25)]
        public void PlayInterstitialsAds()
        {
            this.Publish(new PlayAdRequestSignal() { IsSkippable = true, CustomAdType = CustomAdType.Interstitial, FallbackAdType = UnityAds.AdReward.NoReward });
        }
    }
}