using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;

using Common;
using Common.Query;
using Common.Signal;

namespace Framework
{
    // alias
    using CColor = Framework.Color;
    using UScene = UnityEngine.SceneManagement.Scene;
    using Sirenix.OdinInspector;

    //CLASS BEING USED TO POPULATE THE LIST OF USABLE SCENES, HAS A BUTTON WHICH SELECTS AND UPDATES THE CURRENT LIST BEING USED ON THE SCENE INSTANCE
    [Serializable]
    public class TypeOfSceneClass
    {
        private Scene SceneReference;

        public void SetSceneReference(Scene temp)
        {
            SceneReference = temp;
        }

        [Button(25)]
        private void Select()
        {
            //SceneReference.SceneTypeString = name;
            //SceneReference.UpdateEnum((EScene)(Enum.Parse(typeof(EScene), name)));
            //SceneReference.HideSceneTypeDropdown();
        }

        [DisableContextMenu]
        public string name = "";
    }
}