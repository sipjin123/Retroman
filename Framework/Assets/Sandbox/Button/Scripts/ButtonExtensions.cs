using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Framework;

namespace Sandbox.ButtonSandbox
{
    public static class ButtonExtensions
    {
        public static ButtonType ToButton(this string button)
        {
            Assertion.Assert(ButtonType.ButtonTypeMap.ContainsKey(button), D.ERROR + "Invalid Button:{0}!\n", button);

            return ButtonType.ButtonTypeMap[button];
        }

        public static ButtonType ToButton(this int button)
        {
            Assertion.Assert(ButtonType.ButtonTypeList.Exists(p => p.Value == button), D.ERROR + "Invalid Button:{0}!\n", button);

            return ButtonType.ButtonTypeList.Find(p => p.Value == button);
        }

        public static int ToButtonValue(this string button)
        {
            Assertion.Assert(ButtonType.ButtonTypeMap.ContainsKey(button), D.ERROR + "Invalid Button:{0}!\n", button);

            return ButtonType.ButtonTypeMap[button].Value;
        }
    }
}
