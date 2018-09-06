using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "Vector2Int",
        order = ORDER_BASE + 6)]
    public class Vector2IntVariable : Variable<Vector2Int>
    {
    }
}