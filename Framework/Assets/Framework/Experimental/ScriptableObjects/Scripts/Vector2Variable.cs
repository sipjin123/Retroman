using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "Vector2",
        order = ORDER_BASE + 5)]
    public class Vector2Variable : Variable<Vector2>
    {
    }
}