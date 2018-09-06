using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "Vector3",
        order = ORDER_BASE + 7)]
    public class Vector3Variable : Variable<Vector3>
    {
    }
}