using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using uPromise;

using UniRx;

using Common;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

using Framework;

namespace Sandbox.Popup
{
    public partial class PopupWindow : MonoBehaviour, IPopupWindow
    {
        public ValueDropdownList<string> GetPopups()
        {
            return PopupType.PopupList;
        }
    }
}