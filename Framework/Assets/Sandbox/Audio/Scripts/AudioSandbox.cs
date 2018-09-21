using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;
using UniRx.Triggers;

using uPromise;

using Common;
using Common.Query;

using Framework;

namespace Sandbox.Audio
{
    using FScene = Framework.Scene;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This handles playing of common sound effects like button hovers and clicks.
    /// </summary>
    public class AudioSandbox : SceneObject
    {
        protected override void Start()
        {
            base.Start();
        }
    }
}
