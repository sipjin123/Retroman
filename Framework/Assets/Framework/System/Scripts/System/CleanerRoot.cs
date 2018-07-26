using UnityEngine;

using System;
using System.Collections;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

namespace Framework
{
    public class CleanerRoot : Scene
    {
        protected override void Awake()
        {
            SceneType = EScene.Cleaner;

            base.Awake();

            Debug.Log("[Framework] CleanUpRoot::Awake CLEANUP\n");

            // CleanUp Memory Usage
            Resources.UnloadUnusedAssets();

            // Removed for Assetbundle
            //Caching.CleanCache();
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}