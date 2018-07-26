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
    public class SceneOptionsTest : MonoBehaviour
    {
        /*
        [SerializeField]
        private SceneOptions SceneOptions;
        //*/

        /*
        public static ValueDropdownList<EScene> SceneList;

        public static ValueDropdownList<EScene> GenerateSceneList()
        {
            List<string> scenes = new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkScenes.dat"));
            ValueDropdownList<EScene> dropdown = new ValueDropdownList<EScene>();
            scenes.ForEach(s => dropdown.Add(s, s.ToEnum<EScene>()));

            return dropdown;
        }
        
        [SerializeField]
        [DisableInEditorMode]
        private string _SceneTypeString;
        public string SceneTypeString
        {
            get { return _SceneTypeString; }
            private set { _SceneTypeString = value; }
        }

        private void UpdateSceneTypeString()
        {
            SceneTypeString = SceneType.ToString();
        }
        
        [ValueDropdown("Scenes")]
        [OnValueChanged("UpdateSceneTypeString")]
        public EScene SceneType;

        
        public ValueDropdownList<EScene> Scenes()
        {
            SceneList = SceneList ?? GenerateSceneList();
            return SceneList;
        }
        //*/

        public static ValueDropdownList<string> SceneList;

        public static ValueDropdownList<string> GenerateSceneList()
        {
            List<string> scenes = new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkScenes.dat"));
            ValueDropdownList<string> dropdown = new ValueDropdownList<string>();
            scenes.ForEach(s => dropdown.Add(s));

            return dropdown;
        }
        
        private void UpdateSceneTypeString()
        {
            SceneTypeString = SceneType;
            Scene = SceneType.ToEnum<EScene>();
        }

        public ValueDropdownList<string> Scenes()
        {
            SceneList = SceneList ?? GenerateSceneList();
            return SceneList;
        }

        [SerializeField]
        [HideInInspector]
        [DisableInEditorMode]
        private string SceneTypeString;

        [SerializeField]
        [ValueDropdown("Scenes")]
        [OnValueChanged("UpdateSceneTypeString")]
        private string SceneType;
        
        public EScene Scene { get; private set; }
    }
}