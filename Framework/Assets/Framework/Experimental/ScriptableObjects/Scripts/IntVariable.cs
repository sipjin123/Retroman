using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "int",
        order = ORDER_BASE + 2)]
    public class IntVariable : Variable<int>
    {
    }
}