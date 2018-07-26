using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using uPromise;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Framework
{
    public enum SplashState
    {
        HIDDEN,
        FADE_IN,
        STAY,
        FADE_OUT
    };

    public class SplashRoot : SceneObject
    {
        [SerializeField]
        private bool Done = false;

        [SerializeField]
        private CanvasGroup Group;

        [SerializeField]
        private List<GameObject> Images;

        [SerializeField]
        [Range(0f, 1f)]
        private float Fade = 0.25f;

        [SerializeField]
        [Range(0f, 1f)]
        private float Stay = 0.5f;
        
        private float Alpha = 0f;
        private float Timer = 0f;
        private int Index = 0;
        
        private SplashState State;
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            // prepare the active images
            Images.ForEach(i => i.SetActive(false));
            Group.alpha = 0f;

            StartFade(Index);
        }

        protected override IEnumerator Wait(Deferred def)
        {
#if ENABLE_SPLASH_VIDEO
            while (!Done)
            {
                yield return new WaitForEndOfFrame();
            }
#endif

            // resolve
            yield return null;
            def.Resolve();
        }

        private void Update()
        {
            if (Done)
            {
                return;
            }

            Timer += Time.deltaTime;

            switch (State)
            {
                case SplashState.HIDDEN:
                    if (Index >= Images.Count - 1)
                    {
                        Done = true;
                        this.Publish(new OnSplashDoneSignal());
                    }
                    else
                    {
                        StartFade(Index + 1);
                    }

                    break;

                case SplashState.FADE_IN:
                    if (Timer >= Fade)
                    {
                        SetAlpha(1f);
                        SetState(SplashState.STAY);
                    }
                    else
                    {
                        SetAlpha(Timer / Fade);
                    }

                    break;

                case SplashState.STAY:
                    if (Timer >= Stay)
                    {
                        SetState(SplashState.FADE_OUT);
                    }
                    break;

                case SplashState.FADE_OUT:
                    if (Timer >= Fade)
                    {
                        SetAlpha(0f);
                        SetState(SplashState.HIDDEN);
                    }
                    else
                    {
                        SetAlpha(1f - (Timer / Fade));
                    }
                    break;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SetAlpha(0f);
                SetState(SplashState.HIDDEN);
            }
        }
        
        private void StartFade(int index)
        {
            Images.ForEach(i => i.SetActive(false));
            Images[index].SetActive(true);
            Index = index;
            SetState(SplashState.FADE_IN);
        }

        private void SetAlpha(float a)
        {
            Group.alpha = a;
        }

        private void SetState(SplashState state)
        {
            State = state;
            Timer = 0f;
        }
    }
}

