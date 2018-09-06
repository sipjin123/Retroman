using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Framework;

namespace Sandbox.Popup
{
    public static class PopupExtensions
    {
        public static PopupType ToPopup(this string popup)
        {
            Assertion.Assert(PopupType.PopupTypeMap.ContainsKey(popup), D.ERROR + "Invalid Popup:{0}!\n", popup);

            return PopupType.PopupTypeMap[popup];
        }

        public static PopupType ToPopup(this int popup)
        {
            Assertion.Assert(PopupType.PopupTypeList.Exists(p => p.Value == popup), D.ERROR + "Invalid Popup:{0}!\n", popup);

            return PopupType.PopupTypeList.Find(p => p.Value == popup);
        }

        public static int ToPopupValue(this string popup)
        {
            Assertion.Assert(PopupType.PopupTypeMap.ContainsKey(popup), D.ERROR + "Invalid Popup:{0}!\n", popup);

            return PopupType.PopupTypeMap[popup].Value;
        }
    }
}
