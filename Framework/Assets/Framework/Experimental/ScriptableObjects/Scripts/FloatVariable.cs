using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "float",
        order = ORDER_BASE + 3)]
    public class FloatVariable : Variable<float>
    {
    }
}