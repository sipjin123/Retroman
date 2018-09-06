using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "bool",
        order = ORDER_BASE + 1)]
    public class BoolVariable : Variable<bool>
    {
    }
}