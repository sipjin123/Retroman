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
    
    // alias
    using FScene = Framework.Scene;
    using UScene = UnityEngine.SceneManagement.Scene;
    using UObject = UnityEngine.Object;
    
    public partial class PopupCollectionRoot : FScene
    {
        public ValueDropdownList<string> GetPopups()
        {
            return PopupType.PopupList;
        }
    }
}