using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "Vector4",
        order = ORDER_BASE + 9)]
    public class Vector4Variable : Variable<Vector4>
    {
    }
}